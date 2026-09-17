using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Instagram.ApiModels;

public sealed class InstagramCommentsResponse
{
    [JsonPropertyName("data")]
    public List<InstagramComment> Data { get; set; } = [];

    [JsonPropertyName("paging")]
    public InstagramPaging? Paging { get; set; }
}

public sealed class InstagramComment
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}