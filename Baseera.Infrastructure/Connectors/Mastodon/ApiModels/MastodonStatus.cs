using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Mastodon.ApiModels;

public sealed class MastodonStatus
{
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    public string? Content { get; init; }

    public string? Url { get; init; }

    [JsonPropertyName("replies_count")]
    public int RepliesCount { get; init; }

    [JsonPropertyName("reblogs_count")]
    public int ReblogsCount { get; init; }

    [JsonPropertyName("favourites_count")]
    public int FavouritesCount { get; init; }

    public MastodonAccount? Account { get; init; }

    [JsonPropertyName("media_attachments")]
    public IReadOnlyList<MastodonMediaAttachment> MediaAttachments { get; init; } = [];
}

public sealed class MastodonAccount
{
    public string Id { get; init; } = string.Empty;

    public string? Username { get; init; }

    public string? Acct { get; init; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    public string? Url { get; init; }
}

public sealed class MastodonMediaAttachment
{
    public string Id { get; init; } = string.Empty;

    public string? Type { get; init; }

    public string? Url { get; init; }

    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; init; }
}