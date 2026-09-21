using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeExtraction.Prompts;
using Baseera.Infrastructure.Options;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Type = Google.GenAI.Types.Type;

namespace Baseera.Infrastructure.KnowledgeExtraction.Gemini;

public sealed class GeminiKnowledgeExtractor : IKnowledgeExtractor
{
    private readonly Client _client;
    private readonly GeminiOptions _options;

    public GeminiKnowledgeExtractor(Client client, IOptions<GeminiOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<KnowledgeExtractionResult> ExtractAsync(
        UnifiedRawDocument document,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        var text = BuildExtractionText(document);

        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts =
                [
                    new Part
                    {
                        Text = KnowledgeExtractionPrompt.System
                    }
                ]
            },

            Temperature = 0,

            ResponseMimeType = "application/json",

            ResponseSchema = BuildResponseSchema()
        };

        var response = await _client.Models.GenerateContentAsync(
            model: _options.Model,
            contents: text,
            config: config,
            cancellationToken: cancellationToken);

        var json = response.Candidates[0]
            .Content!
            .Parts![0]
            .Text!;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(
            new JsonStringEnumConverter());

        var result =
            JsonSerializer.Deserialize<KnowledgeExtractionResult>(
                json,
                options);

        return result
            ?? throw new InvalidOperationException(
                "Gemini returned an empty knowledge extraction result.");
    }


    private static string BuildExtractionText(
    UnifiedRawDocument document)
    {
        var parts = new List<string>();

        parts.Add($"Platform: {document.Platform}");

        if (!string.IsNullOrWhiteSpace(document.Author))
            parts.Add($"Author: {document.Author}");

        if (!string.IsNullOrWhiteSpace(document.Title))
            parts.Add($"Title: {document.Title}");

        if (!string.IsNullOrWhiteSpace(document.Body))
            parts.Add($"Body: {document.Body}");

        if (document.Comments.Count > 0)
        {
            parts.Add("Comments:");

            foreach (var comment in document.Comments)
            {
                parts.Add(
                    $"- {comment.Author}: {comment.Body}");
            }
        }

        return string.Join(
            System.Environment.NewLine,
            parts);
    }

    private static Schema BuildResponseSchema()
    {
        var entitySchema = new Schema
        {
            Type = Type.Object,

            Properties = new Dictionary<string, Schema>
            {
                ["id"] = new Schema
                {
                    Type = Type.String
                },
                ["name"] = new Schema
                {
                    Type = Type.String
                },

                ["type"] = new Schema
                {
                    Type = Type.String,
                    Enum = new List<string>
                {
                    "Brand",
                    "Product",
                    "Service",
                    "Feature",
                    "Topic",
                    "Person",
                    "Campaign",
                    "Event"
                }
                },

                ["description"] = new Schema
                {
                    Type = Type.String,
                    Nullable = true
                },

                ["evidence"] = new Schema
                {
                    Type = Type.String,
                    Nullable = true
                },

                ["confidence"] = new Schema
                {
                    Type = Type.Number
                }
            },

            Required =
            [
                "id",
                "name",
                "type",
                "description",
                "evidence",
                "confidence"
            ],

            PropertyOrdering =
            [
                "id",
                "name",
                "type",
                "description",
                "evidence",
                "confidence"
            ]
        };

        var relationshipSchema = new Schema
        {
            Type = Type.Object,

            Properties = new Dictionary<string, Schema>
            {
                ["sourceId"] = new Schema
                {
                    Type = Type.String
                },

                ["relationship"] = new Schema
                {
                    Type = Type.String,
                    Enum = new List<string>
                {
                    "HasProduct",
                    "HasService",
                    "HasFeature",
                    "HasTopic",
                    "RunsCampaign",
                    "HasEvent",
                    "LedBy",
                    "RelatedTo"
                }
                },

                ["targetId"] = new Schema
                {
                    Type = Type.String
                },

                ["evidence"] = new Schema
                {
                    Type = Type.String,
                    Nullable = true
                },

                ["confidence"] = new Schema
                {
                    Type = Type.Number
                }
            },

            Required =
            [
                "sourceId",
                "relationship",
                "targetId",
                "evidence",
                "confidence"
            ],

            PropertyOrdering =
            [
                "sourceId",
                "relationship",
                "targetId",
                "evidence",
                "confidence"
            ]
        };

        return new Schema
        {
            Type = Type.Object,

            Properties = new Dictionary<string, Schema>
            {
                ["entities"] = new Schema
                {
                    Type = Type.Array,
                    Items = entitySchema
                },

                ["relationships"] = new Schema
                {
                    Type = Type.Array,
                    Items = relationshipSchema
                }
            },

            Required =
            [
                "entities",
                "relationships"
            ],

            PropertyOrdering =
            [
                "entities",
                "relationships"
            ]
        };
    }
}