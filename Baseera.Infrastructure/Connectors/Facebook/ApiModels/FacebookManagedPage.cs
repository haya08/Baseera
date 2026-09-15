using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Facebook.ApiModels;

public sealed class FacebookManagedPagesResponse
{
    public IReadOnlyList<FacebookManagedPage> Data { get; init; } = [];
}

public sealed class FacebookManagedPage
{
    public string Id { get; init; } = string.Empty;

    public string? Name { get; init; }

    [JsonPropertyName("link")]
    public string? Link { get; init; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}