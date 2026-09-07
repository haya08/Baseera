using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgeExtraction.Prompts;
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
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));
        }

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

    private static Schema BuildResponseSchema()
    {
        var entitySchema = new Schema
        {
            Type = Type.Object,

            Properties = new Dictionary<string, Schema>
            {
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
                "name",
                "type",
                "description",
                "evidence",
                "confidence"
            ],

            PropertyOrdering =
            [
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
                ["source"] = new Schema
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

                ["target"] = new Schema
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
                "source",
                "relationship",
                "target",
                "evidence",
                "confidence"
            ],

            PropertyOrdering =
            [
                "source",
                "relationship",
                "target",
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