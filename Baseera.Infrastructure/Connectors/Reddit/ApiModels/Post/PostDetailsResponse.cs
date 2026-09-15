using System.Text.Json.Serialization;
namespace Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;

public sealed class PostDetailsResponse
{
    public bool Success { get; init; }

    public PostDetailsData Data { get; init; } = new();

    public int CreditsUsed { get; init; }

    public bool CacheHit { get; init; }

    public DateTimeOffset Timestamp { get; init; }
}

public sealed class PostDetailsData
{
    [JsonPropertyName("post_id")]
    public string PostId { get; init; } = string.Empty;

    public string? Title { get; init; }

    public string? Author { get; init; }

    public string? Subreddit { get; init; }

    public int Score { get; init; }

    [JsonPropertyName("upvote_ratio")]
    public double UpvoteRatio { get; init; }

    [JsonPropertyName("num_comments")]
    public int NumComments { get; init; }

    [JsonPropertyName("created_utc")]
    public double CreatedUtc { get; init; }

    public string? Permalink { get; init; }

    public string? Url { get; init; }

    [JsonPropertyName("selftext")]
    public string? SelfText { get; init; }

    [JsonPropertyName("selftext_html")]
    public string? SelfTextHtml { get; init; }

    [JsonPropertyName("is_self")]
    public bool IsSelf { get; init; }

    [JsonPropertyName("is_video")]
    public bool IsVideo { get; init; }

    [JsonPropertyName("is_nsfw")]
    public bool IsNsfw { get; init; }

    [JsonPropertyName("is_spoiler")]
    public bool IsSpoiler { get; init; }

    [JsonPropertyName("is_stickied")]
    public bool IsStickied { get; init; }

    [JsonPropertyName("link_flair_text")]
    public string? LinkFlairText { get; init; }

    public string? Thumbnail { get; init; }

    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; init; }

    public string? Domain { get; init; }

    [JsonPropertyName("awards_count")]
    public int AwardsCount { get; init; }
}