using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment
{
    public sealed class PostCommentResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("data")]
        public required PostCommentsData Data { get; init; }

        [JsonPropertyName("credits_used")]
        public int CreditsUsed { get; init; }

        [JsonPropertyName("cache_hit")]
        public bool CacheHit { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }
    }

    public sealed class PostCommentsData
    {
        [JsonPropertyName("post_id")]
        public required string PostId { get; init; }

        [JsonPropertyName("post_title")]
        public required string PostTitle { get; init; }

        [JsonPropertyName("subreddit")]
        public required string Subreddit { get; init; }

        [JsonPropertyName("comments")]
        public required IReadOnlyList<RedditCommentModel> Comments { get; init; }

        [JsonPropertyName("total_comments")]
        public int TotalComments { get; init; }
    }

    public sealed class RedditCommentModel
    {
        [JsonPropertyName("comment_id")]
        public required string CommentId { get; init; }

        [JsonPropertyName("author")]
        public required string Author { get; init; }

        [JsonPropertyName("body")]
        public required string Body { get; init; }

        [JsonPropertyName("body_html")]
        public string? BodyHtml { get; init; }

        [JsonPropertyName("score")]
        public int Score { get; init; }

        [JsonPropertyName("created_utc")]
        public DateTimeOffset CreatedUtc { get; init; }

        [JsonPropertyName("is_submitter")]
        public bool IsSubmitter { get; init; }

        [JsonPropertyName("is_stickied")]
        public bool IsStickied { get; init; }

        [JsonPropertyName("parent_id")]
        public required string ParentId { get; init; }

        [JsonPropertyName("depth")]
        public int Depth { get; init; }

        [JsonPropertyName("awards_count")]
        public int AwardsCount { get; init; }

        [JsonPropertyName("reply_count")]
        public int ReplyCount { get; init; }
    }
}
