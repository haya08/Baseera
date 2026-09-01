using DotNetEnv;
using Google.GenAI;
using Google.GenAI.Types;
using Type = Google.GenAI.Types.Type;

Env.Load();

var apiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "GEMINI_API_KEY is not configured.");
}

using var client = new Client(apiKey: apiKey);

var schema = new Schema
{
    Type = Type.Object,

    Properties = new Dictionary<string, Schema>
    {
        ["brand"] = new Schema
        {
            Type = Type.String
        },

        ["product"] = new Schema
        {
            Type = Type.String
        }
    },

    Required =
    [
        "brand",
        "product"
    ],

    PropertyOrdering =
    [
        "brand",
        "product"
    ]
};

const string systemInstruction = """
Extract the brand and product from the provided social media post.

Return only the fields defined by the response schema.
Do not use external knowledge.
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

    ResponseSchema = schema
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-3.6-flash",
    contents: post,
    config: config);

var resultText =
    response.Candidates[0]
        .Content!
        .Parts![0]
        .Text!;

Console.WriteLine("================================");
Console.WriteLine("GEMINI RESPONSE");
Console.WriteLine("================================");
Console.WriteLine(resultText);