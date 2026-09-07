using Baseera.Core.Documents.Enums;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;

using Microsoft.Extensions.Options;

using Neo4j.Driver;

namespace Baseera.Infrastructure.KnowledgeResolution.Neo4j;

public sealed class Neo4jCandidateProvider : ICandidateProvider
{
    private readonly IDriver _driver;
    private readonly Neo4jOptions _options;
    private readonly IEmbeddingGenerator _embeddingGenerator;

    private static readonly IReadOnlyDictionary<EntityType, string> VectorIndexes =
        new Dictionary<EntityType, string>
        {
            [EntityType.Brand] = "brand_embedding_index",
            [EntityType.Product] = "product_embedding_index",
            [EntityType.Service] = "service_embedding_index",
            [EntityType.Feature] = "feature_embedding_index",
            [EntityType.Topic] = "topic_embedding_index",
            [EntityType.Person] = "person_embedding_index",
            [EntityType.Campaign] = "campaign_embedding_index",
            [EntityType.Event] = "event_embedding_index"
        };

    private static readonly IReadOnlyDictionary<EntityType, string> FullTextIndexes =
        new Dictionary<EntityType, string>
        {
            [EntityType.Brand] = "brand_fulltext_index",
            [EntityType.Product] = "product_fulltext_index",
            [EntityType.Service] = "service_fulltext_index",
            [EntityType.Feature] = "feature_fulltext_index",
            [EntityType.Topic] = "topic_fulltext_index",
            [EntityType.Person] = "person_fulltext_index",
            [EntityType.Campaign] = "campaign_fulltext_index",
            [EntityType.Event] = "event_fulltext_index"
        };

    public Neo4jCandidateProvider(
        IDriver driver,
        IOptions<Neo4jOptions> options,
        IEmbeddingGenerator embeddingGenerator)
    {
        _driver = driver;
        _options = options.Value;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<IReadOnlyList<ResolutionCandidate>> GetCandidatesAsync(
        ExtractedEntity entity,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (topK <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topK),
                "Top-K must be greater than zero.");
        }

        if (!IsSupportedEntityType(entity.Type))
        {
            return [];
        }

        var normalizedName = Normalize(entity.Name);

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return [];
        }

        // 1. Exact match
        var exactMatch = await FindExactMatchAsync(
            entity.Type,
            normalizedName,
            cancellationToken);

        if (exactMatch is not null)
        {
            return [exactMatch];
        }

        // 2. Alias match
        var aliasMatches = await FindAliasMatchesAsync(
            entity.Type,
            normalizedName,
            cancellationToken);

        if (aliasMatches.Count > 0)
        {
            return aliasMatches
                .Take(topK)
                .ToList();
        }

        // 3. Generate embedding
        var embeddingText = BuildEmbeddingText(entity);

        var embedding = await _embeddingGenerator.GenerateAsync(
            embeddingText,
            cancellationToken);

        // Retrieve more candidates than we finally need
        var retrievalK = Math.Max(topK * 2, 10);

        // 4. Full-text search
        var textCandidates = await SearchFullTextAsync(
            entity.Type,
            entity.Name,
            retrievalK,
            cancellationToken);

        // 5. Vector search
        var vectorCandidates = await SearchVectorAsync(
            entity.Type,
            embedding,
            retrievalK,
            cancellationToken);

        // 6. Merge + rank
        return MergeAndRank(
            textCandidates,
            vectorCandidates,
            topK);
    }

    private async Task<ResolutionCandidate?> FindExactMatchAsync(
        EntityType entityType,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        if (!VectorIndexes.ContainsKey(entityType))
        {
            return null;
        }

        var label = entityType.ToString();

        var query = $"""
            MATCH (e:{label})
            WHERE e.normalizedName = $normalizedName

            RETURN
                e.id AS id,
                e.name AS name,
                e.description AS description

            LIMIT 1
            """;

        var parameters = new Dictionary<string, object>
        {
            ["normalizedName"] = normalizedName
        };

        await using var session = CreateSession();

        var result = await session.RunAsync(
            query,
            parameters);

        var record = await result.SingleOrDefaultAsync();

        if (record is null)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new ResolutionCandidate
        {
            EntityId = Guid.Parse(record["id"].As<string>()),
            Name = record["name"].As<string>(),
            Type = entityType,
            Description = record["description"].As<string?>(),
            MatchType = CandidateMatchType.Exact,
            FinalScore = 1.0
        };
    }

    private async Task<IReadOnlyList<ResolutionCandidate>> FindAliasMatchesAsync(
        EntityType entityType,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        if (!VectorIndexes.ContainsKey(entityType))
        {
            return [];
        }

        var label = entityType.ToString();

        var query = $"""
            MATCH (e:{label})-[:HAS_ALIAS]->(a:Alias)
            WHERE a.normalizedValue = $normalizedName

            RETURN
                e.id AS id,
                e.name AS name,
                e.description AS description
            """;

        var parameters = new Dictionary<string, object>
        {
            ["normalizedName"] = normalizedName
        };

        await using var session = CreateSession();

        var result = await session.RunAsync(
            query,
            parameters);

        var candidates = new List<ResolutionCandidate>();

        await result.ForEachAsync(record =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            candidates.Add(new ResolutionCandidate
            {
                EntityId = Guid.Parse(
                    record["id"].As<string>()),

                Name = record["name"].As<string>(),

                Type = entityType,

                Description = record["description"].As<string?>(),

                MatchType = CandidateMatchType.Alias,

                FinalScore = 1.0
            });
        });

        return candidates;
    }

    private async Task<IReadOnlyList<ResolutionCandidate>> SearchFullTextAsync(
        EntityType entityType,
        string queryText,
        int topK,
        CancellationToken cancellationToken)
    {
        if (!FullTextIndexes.TryGetValue(
                entityType,
                out var indexName))
        {
            return [];
        }

        var query = """
            CALL db.index.fulltext.queryNodes(
                $indexName,
                $queryText
            )
            YIELD node, score

            RETURN
                node.id AS id,
                node.name AS name,
                node.description AS description,
                score

            ORDER BY score DESC
            LIMIT $topK
            """;

        var parameters = new Dictionary<string, object>
        {
            ["indexName"] = indexName,
            ["queryText"] = queryText,
            ["topK"] = topK
        };

        await using var session = CreateSession();

        var result = await session.RunAsync(
            query,
            parameters);

        var candidates = new List<ResolutionCandidate>();

        await result.ForEachAsync(record =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            candidates.Add(new ResolutionCandidate
            {
                EntityId = Guid.Parse(
                    record["id"].As<string>()),

                Name = record["name"].As<string>(),

                Type = entityType,

                Description = record["description"].As<string?>(),

                TextScore = record["score"].As<double>(),

                MatchType = CandidateMatchType.FullText
            });
        });

        return candidates;
    }

    private async Task<IReadOnlyList<ResolutionCandidate>> SearchVectorAsync(
        EntityType entityType,
        IReadOnlyList<double> embedding,
        int topK,
        CancellationToken cancellationToken)
    {
        if (!VectorIndexes.TryGetValue(
                entityType,
                out var indexName))
        {
            return [];
        }

        var query = """
            CALL db.index.vector.queryNodes(
                $indexName,
                $topK,
                $embedding
            )
            YIELD node, score

            RETURN
                node.id AS id,
                node.name AS name,
                node.description AS description,
                score

            ORDER BY score DESC
            """;

        var parameters = new Dictionary<string, object>
        {
            ["indexName"] = indexName,
            ["topK"] = topK,
            ["embedding"] = embedding.ToArray()
        };

        await using var session = CreateSession();

        var result = await session.RunAsync(
            query,
            parameters);

        var candidates = new List<ResolutionCandidate>();

        await result.ForEachAsync(record =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            candidates.Add(new ResolutionCandidate
            {
                EntityId = Guid.Parse(
                    record["id"].As<string>()),

                Name = record["name"].As<string>(),

                Type = entityType,

                Description = record["description"].As<string?>(),

                VectorScore = record["score"].As<double>(),

                MatchType = CandidateMatchType.Vector
            });
        });

        return candidates;
    }

    private static IReadOnlyList<ResolutionCandidate> MergeAndRank(
        IReadOnlyList<ResolutionCandidate> textCandidates,
        IReadOnlyList<ResolutionCandidate> vectorCandidates,
        int topK)
    {
        var rankings = new Dictionary<Guid, CandidateRanking>();

        // Full-text results
        for (var i = 0; i < textCandidates.Count; i++)
        {
            var candidate = textCandidates[i];

            if (!rankings.TryGetValue(
                    candidate.EntityId,
                    out var ranking))
            {
                ranking = new CandidateRanking(candidate);

                rankings[candidate.EntityId] = ranking;
            }

            ranking.TextRank = i + 1;
            ranking.TextScore = candidate.TextScore;
        }

        // Vector results
        for (var i = 0; i < vectorCandidates.Count; i++)
        {
            var candidate = vectorCandidates[i];

            if (!rankings.TryGetValue(
                    candidate.EntityId,
                    out var ranking))
            {
                ranking = new CandidateRanking(candidate);

                rankings[candidate.EntityId] = ranking;
            }

            ranking.VectorRank = i + 1;
            ranking.VectorScore = candidate.VectorScore;
        }

        const double rrfConstant = 60.0;

        return rankings.Values
            .Select(ranking =>
            {
                var score = 0.0;

                if (ranking.TextRank.HasValue)
                {
                    score += 1.0 /
                             (rrfConstant + ranking.TextRank.Value);
                }

                if (ranking.VectorRank.HasValue)
                {
                    score += 1.0 /
                             (rrfConstant + ranking.VectorRank.Value);
                }

                ranking.FinalScore = score;

                return ranking.ToResolutionCandidate();
            })
            .OrderByDescending(candidate => candidate.FinalScore)
            .Take(topK)
            .ToList();
    }

    private IAsyncSession CreateSession()
    {
        return _driver.AsyncSession(
            sessionConfig =>
                sessionConfig.WithDatabase(
                    _options.Database));
    }

    private static string BuildEmbeddingText(
        ExtractedEntity entity)
    {
        return $"""
            Name: {entity.Name}
            Type: {entity.Type}
            Description: {entity.Description ?? string.Empty}
            """;
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant();
    }

    private static bool IsSupportedEntityType(
        EntityType entityType)
    {
        return VectorIndexes.ContainsKey(entityType);
    }

    private sealed class CandidateRanking
    {
        private readonly ResolutionCandidate _candidate;

        public CandidateRanking(
            ResolutionCandidate candidate)
        {
            _candidate = candidate;
        }

        public int? TextRank { get; set; }

        public int? VectorRank { get; set; }

        public double? TextScore { get; set; }

        public double? VectorScore { get; set; }

        public double FinalScore { get; set; }

        public ResolutionCandidate ToResolutionCandidate()
        {
            _candidate.TextScore = TextScore;
            _candidate.VectorScore = VectorScore;
            _candidate.FinalScore = FinalScore;

            _candidate.MatchType =
                TextRank.HasValue && VectorRank.HasValue
                    ? CandidateMatchType.Hybrid
                    : TextRank.HasValue
                        ? CandidateMatchType.FullText
                        : CandidateMatchType.Vector;

            return _candidate;
        }
    }
}