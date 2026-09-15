using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Bluesky.ApiModels;

public sealed class BlueskyFeedResponse
{
    public IReadOnlyList<BlueskyFeedItem> Feed { get; init; } = [];

    public string? Cursor { get; init; }
}

public sealed class BlueskyFeedItem
{
    public BlueskyPostView? Post { get; init; }

    public BlueskyReason? Reason { get; init; }
}

public sealed class BlueskyPostView
{
    public string Uri { get; init; } = string.Empty;

    public string Cid { get; init; } = string.Empty;

    public BlueskyRecord? Record { get; init; }

    public BlueskyAuthor? Author { get; init; }

    [JsonPropertyName("replyCount")]
    public int ReplyCount { get; init; }

    [JsonPropertyName("repostCount")]
    public int RepostCount { get; init; }

    [JsonPropertyName("likeCount")]
    public int LikeCount { get; init; }

    public string? IndexedAt { get; init; }
}

public sealed class BlueskyRecord
{
    public string? Text { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class BlueskyAuthor
{
    public string Did { get; init; } = string.Empty;

    public string Handle { get; init; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    public string? Avatar { get; init; }

    public string? Viewer { get; init; }
}

public sealed class BlueskyReason
{
    public string? Type { get; init; }
}