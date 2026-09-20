namespace Baseera.Infrastructure.Options
{
    public sealed class GeminiOptions
    {
        public string ApiKey { get; set; } = null!;

        public string Model { get; set; } = "gemini-3.1-flash-lite";
    }
}
