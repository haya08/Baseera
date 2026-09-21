using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Bluesky.ApiModels;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Bluesky.Mappers;

public sealed class BlueskyMapper : IBlueskyMapper
{
    public UnifiedRawDocument Map(BlueskyFeedItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(item.Post);

        var post = item.Post;

        return new UnifiedRawDocument
        {
            ExternalId = post.Uri,

            Platform = Platform.Bluesky,

            Title = null,

            Body = post.Record?.Text,

            Comments = [],

            Author = post.Author?.DisplayName
                     ?? post.Author?.Handle,

            Url = BuildPostUrl(post.Author?.Handle, post.Uri),

            PublishedAt = post.Record?.CreatedAt
                          ?? DateTimeOffset.UtcNow,

            CollectedAt = DateTimeOffset.UtcNow,

            Metadata = new Dictionary<string, string>
            {
                ["author_did"] = post.Author?.Did ?? string.Empty,
                ["handle"] = post.Author?.Handle ?? string.Empty,
                ["like_count"] = post.LikeCount.ToString(),
                ["repost_count"] = post.RepostCount.ToString(),
                ["reply_count"] = post.ReplyCount.ToString(),
                ["cid"] = post.Cid
            },

            RawJson = JsonSerializer.Serialize(item)
        };
    }

    private static string? BuildPostUrl(
        string? handle,
        string uri)
    {
        if (string.IsNullOrWhiteSpace(handle))
            return null;

        var rkey = uri.Split('/').LastOrDefault();

        return string.IsNullOrWhiteSpace(rkey)
            ? null
            : $"https://bsky.app/profile/{handle}/post/{rkey}";
    }
}