using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Core.KnowledgeResolution.Enums;
using Baseera.Core.KnowledgeResolution.Models;
using Baseera.Infrastructure.KnowledgeResolution.Gemini.Models;
using Baseera.Infrastructure.Options;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Baseera.Infrastructure.KnowledgeResolution.Gemini
{
    public sealed class GeminiResolutionJudge : IResolutionJudge
    {
        private readonly Client _client;
        private readonly GeminiOptions _options;

        public GeminiResolutionJudge(
            Client client,
            IOptions<GeminiOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<ResolutionDecision> JudgeAsync(
            ExtractedEntity entity,
            IReadOnlyList<ResolutionCandidate> candidates,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(candidates);

            if (candidates.Count == 0)
            {
                return new ResolutionDecision
                {
                    Decision = ResolutionDecisionType.NoMatch,
                    Confidence = 1.0,
                    Reason = "No candidates were provided."
                };
            }

            var prompt = BuildPrompt(
                entity,
                candidates);

            var config = BuildConfig();

            var response = await _client.Models.GenerateContentAsync(
                model: _options.Model,
                contents: prompt,
                config: config,
                cancellationToken: cancellationToken);

            var json = ExtractResponseText(response);

            Console.WriteLine("RAW JSON:");
            Console.WriteLine(json);
            Console.WriteLine($"FIRST CHAR: {(int)json[0]} '{json[0]}'");
            Console.WriteLine($"LAST CHAR: {(int)json[^1]} '{json[^1]}'");

            var geminiDecision =
                JsonSerializer.Deserialize<GeminiResolutionDecision>(
                    json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (geminiDecision is null)
            {
                throw new InvalidOperationException(
                    "Gemini returned an invalid resolution response.");
            }

            return MapDecision(
                geminiDecision,
                candidates);
        }

        private static GenerateContentConfig BuildConfig()
        {
            return new GenerateContentConfig
            {
                ResponseMimeType = "application/json",

                ResponseJsonSchema = CreateResponseSchema(),

                Temperature = 0.1f,

                MaxOutputTokens = 512
            };
        }

        private static JsonNode CreateResponseSchema()
        {
            return JsonNode.Parse("""
        {
            "type": "object",
            "properties": {
                "decision": {
                    "type": "string",
                    "enum": [
                        "Match",
                        "NoMatch",
                        "Ambiguous"
                    ]
                },
                "candidateEntityId": {
                    "type": [
                        "string",
                        "null"
                    ]
                },
                "confidence": {
                    "type": "number"
                },
                "reason": {
                    "type": [
                        "string",
                        "null"
                    ]
                }
            },
            "required": [
                "decision",
                "candidateEntityId",
                "confidence",
                "reason"
            ]
        }
        """)!;
        }

        private static string BuildPrompt(
            ExtractedEntity entity,
            IReadOnlyList<ResolutionCandidate> candidates)
        {
            var candidateText = string.Join(
                "\n\n",
                candidates.Select((candidate, index) => $"""
                Candidate {index + 1}
                ID: {candidate.EntityId}
                Name: {candidate.Name}
                Type: {candidate.Type}
                Description: {candidate.Description ?? "None"}
                """));

            return $"""
            You are an entity resolution judge for a brand
            knowledge graph.

            Your task is to determine whether the extracted entity
            refers to one of the provided candidate entities.

            Rules:

            1. Only select a candidate from the provided candidates.
            2. Never invent a candidate.
            3. The entity type must match.
            4. Consider names, descriptions, aliases,
               abbreviations, spelling variations, and naming variations.
            5. Similar names alone are not sufficient evidence.
            6. Return "Match" only when one candidate clearly refers
               to the same real-world entity.
            7. Return "NoMatch" when none of the candidates refers
               to the extracted entity.
            8. Return "Ambiguous" when multiple candidates are plausible
               or the available evidence is insufficient.
            9. If the decision is "Match", candidateEntityId must be
               the ID of the selected candidate.
            10. If the decision is "NoMatch" or "Ambiguous",
                candidateEntityId must be null.
            11. Confidence must be between 0 and 1.

            Extracted Entity:

            ID: {entity.Id}
            Name: {entity.Name}
            Type: {entity.Type}
            Description: {entity.Description ?? "None"}

            Candidates:

            {candidateText}
            """;
        }

        private static string ExtractResponseText(
            GenerateContentResponse response)
        {
            var text = response.Candidates?
                .FirstOrDefault()?
                .Content?
                .Parts?
                .FirstOrDefault()?
                .Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty resolution response.");
            }

            return text;
        }

        private static ResolutionDecision MapDecision(
            GeminiResolutionDecision geminiDecision,
            IReadOnlyList<ResolutionCandidate> candidates)
        {
            if (!Enum.TryParse<ResolutionDecisionType>(
                    geminiDecision.Decision,
                    ignoreCase: true,
                    out var decision))
            {
                throw new InvalidOperationException(
                    $"Invalid resolution decision: " +
                    $"{geminiDecision.Decision}");
            }

            if (geminiDecision.Confidence is < 0 or > 1)
            {
                throw new InvalidOperationException(
                    "Gemini returned an invalid confidence score.");
            }

            Guid? candidateId = null;

            if (decision == ResolutionDecisionType.Match)
            {
                if (!Guid.TryParse(
                        geminiDecision.CandidateEntityId,
                        out var parsedId))
                {
                    throw new InvalidOperationException(
                        "Gemini returned Match without a valid candidate ID.");
                }

                var candidateExists = candidates.Any(
                    c => c.EntityId == parsedId);

                if (!candidateExists)
                {
                    throw new InvalidOperationException(
                        "Gemini selected a candidate that was not provided.");
                }

                candidateId = parsedId;
            }

            return new ResolutionDecision
            {
                Decision = decision,
                CandidateEntityId = candidateId,
                Confidence = geminiDecision.Confidence,
                Reason = geminiDecision.Reason
            };
        }
    }
}
