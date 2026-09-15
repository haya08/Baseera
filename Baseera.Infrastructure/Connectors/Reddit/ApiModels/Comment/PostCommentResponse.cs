using System.Text.Json.Serialization;
namespace Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;


public sealed class PostCommentResponse
{
    public bool Success { get; init; }

    public PostCommentData Data { get; init; } = new();

    public int CreditsUsed { get; init; }

    public bool CacheHit { get; init; }

    public DateTimeOffset Timestamp { get; init; }
}

public sealed class PostCommentData
{
    public string PostId { get; init; } = string.Empty;

    public string? PostTitle { get; init; }

    public string? Subreddit { get; init; }

    public IReadOnlyList<RedditComment> Comments { get; init; } = [];

    public int TotalComments { get; init; }
}

public sealed class RedditComment
{
    [JsonPropertyName("comment_id")]
    public string CommentId { get; init; } = string.Empty;

    public string? Author { get; init; }

    public string? Body { get; init; }

    [JsonPropertyName("body_html")]
    public string? BodyHtml { get; init; }

    public int Score { get; init; }

    [JsonPropertyName("created_utc")]
    public double CreatedUtc { get; init; }

    [JsonPropertyName("is_submitter")]
    public bool IsSubmitter { get; init; }

    [JsonPropertyName("is_stickied")]
    public bool IsStickied { get; init; }

    [JsonPropertyName("parent_id")]
    public string? ParentId { get; init; }

    public int Depth { get; init; }

    [JsonPropertyName("awards_count")]
    public int AwardsCount { get; init; }

    [JsonPropertyName("reply_count")]
    public int ReplyCount { get; init; }
}