using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post
{
    public class PostDetailsResponse
    {
        [JsonPropertyName("success")]
        public required bool Success { get; init; }

        [JsonPropertyName("data")]
        public required PostDetailsData Data { get; init; }

        [JsonPropertyName("credits_used")]
        public int CreditsUsed { get; init; }

        [JsonPropertyName("cache_hit")]
        public bool CacheHit { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }
    }

    public sealed class PostDetailsData
    {
        [JsonPropertyName("post_id")]
        public required string PostId { get; init; }

        [JsonPropertyName("title")]
        public required string Title { get; init; }

        [JsonPropertyName("author")]
        public required string Author { get; init; }

        [JsonPropertyName("subreddit")]
        public required string Subreddit { get; init; }

        [JsonPropertyName("score")]
        public int Score { get; init; }

        [JsonPropertyName("upvote_ratio")]
        public double UpvoteRatio { get; init; }

        [JsonPropertyName("num_comments")]
        public int NumComments { get; init; }

        [JsonPropertyName("created_utc")]
        public DateTimeOffset CreatedUtc { get; init; }

        [JsonPropertyName("permalink")]
        public required string Permalink { get; init; }

        [JsonPropertyName("url")]
        public required string Url { get; init; }

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

        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; init; }

        [JsonPropertyName("media_url")]
        public string? MediaUrl { get; init; }

        [JsonPropertyName("domain")]
        public required string Domain { get; init; }

        [JsonPropertyName("awards_count")]
        public int AwardsCount { get; init; }
    }
}
