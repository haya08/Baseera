using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Instagram.ApiModels;

public sealed class InstagramMediaResponse
{
    [JsonPropertyName("data")]
    public List<InstagramMedia> Data { get; set; } = [];

    [JsonPropertyName("paging")]
    public InstagramPaging? Paging { get; set; }
}

public sealed class InstagramMedia
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("like_count")]
    public int? LikeCount { get; set; }

    [JsonPropertyName("comments_count")]
    public int? CommentsCount { get; set; }
}

public sealed class InstagramPaging
{
    [JsonPropertyName("next")]
    public string? Next { get; set; }
}