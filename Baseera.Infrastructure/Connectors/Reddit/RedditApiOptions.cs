namespace Baseera.Infrastructure.Connectors.Reddit;

public sealed class RedditApiOptions
{
    public string BaseUrl { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    public string ApiHost { get; init; } = string.Empty;
}