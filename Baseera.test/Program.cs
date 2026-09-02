using Baseera.Core.Documents.Models;
using DotNetEnv;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using Type = Google.GenAI.Types.Type;
Env.Load();

var apiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "GEMINI_API_KEY was not found.");
}

using var client = new Client(apiKey: apiKey);

const string systemInstruction = """
You are the Knowledge Extraction component of Baseera.

Your task is to extract structured knowledge from a social media post
according to the Baseera Knowledge Schema (BSKO).

Extract only information that is explicitly stated or strongly supported
by the provided text.

Do not use external knowledge.
Do not invent entities, relationships, descriptions, or facts.

Entities may belong to the following types:
- Brand
- Product
- Service
- Feature
- Topic
- Person
- Campaign
- Event

Relationships may belong to the following types:
- HasProduct
- HasService
- HasFeature
- HasTopic
- RunsCampaign
- HasEvent
- LedBy
- RelatedTo

For every extracted entity:
- provide its name
- assign exactly one BSKO entity type
- provide a concise description when supported by the text
- provide evidence from the text when available
- provide a confidence score between 0 and 1

For every extracted relationship:
- identify the source entity
- identify the relationship type
- identify the target entity
- provide evidence from the text when available
- provide a confidence score between 0 and 1

Do not create relationships unless they are supported by the text.

Return the result according to the provided response schema.
""";

const string post = """
Nike introduces the new Nike Pegasus running shoe.
The Pegasus is designed for everyday runners and features React Foam
for a responsive and comfortable ride.
""";

var config = new GenerateContentConfig
{
    SystemInstruction = new Content
    {
        Parts =
        [
            new Part
            {
                Text = systemInstruction
            }
        ]
    },

    Temperature = 0,

    ResponseMimeType = "application/json",

    ResponseSchema = BuildResponseSchema()
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-3.8-flash",
    contents: post,
    config: config);

var resultText =
    response.Candidates[0]
        .Content!
        .Parts![0]
        .Text!;

Console.WriteLine("================================");
Console.WriteLine("RAW GEMINI RESPONSE");
Console.WriteLine("================================");
Console.WriteLine(resultText);

var extractionResult =
    JsonSerializer.Deserialize<KnowledgeExtractionResult>(
        resultText,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        });

Console.WriteLine();
Console.WriteLine("================================");
Console.WriteLine("ENTITIES");
Console.WriteLine("================================");

foreach (var entity in extractionResult!.Entities)
{
    Console.WriteLine(
        $"{entity.Name} | {entity.Type} | {entity.Confidence}");
}

Console.WriteLine();
Console.WriteLine("================================");
Console.WriteLine("RELATIONSHIPS");
Console.WriteLine("================================");

foreach (var relationship in extractionResult.Relationships)
{
    Console.WriteLine(
        $"{relationship.Source} " +
        $"--[{relationship.Relationship}]--> " +
        $"{relationship.Target} " +
        $"| {relationship.Confidence}");
}


static Schema BuildResponseSchema()
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
                Enum =
                [
                    "Brand",
                    "Product",
                    "Service",
                    "Feature",
                    "Topic",
                    "Person",
                    "Campaign",
                    "Event"
                ]
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
                Enum =
                [
                    "HasProduct",
                    "HasService",
                    "HasFeature",
                    "HasTopic",
                    "RunsCampaign",
                    "HasEvent",
                    "LedBy",
                    "RelatedTo"
                ]
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