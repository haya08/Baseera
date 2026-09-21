using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Mastodon.ApiModels;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Mastodon.Mappers;

public sealed class MastodonMapper : IMastodonMapper
{
    public UnifiedRawDocument Map(MastodonStatus status)
    {
        ArgumentNullException.ThrowIfNull(status);

        var author =
            status.Account?.DisplayName
            ?? status.Account?.Username
            ?? status.Account?.Acct;

        return new UnifiedRawDocument
        {
            ExternalId = status.Id,

            Platform = Platform.Mastodon,

            Title = null,

            Body = status.Content,

            Comments = [],

            Author = author,

            Url = status.Url,

            PublishedAt = status.CreatedAt,

            CollectedAt = DateTimeOffset.UtcNow,

            Metadata = new Dictionary<string, string>
            {
                ["replies_count"] = status.RepliesCount.ToString(),
                ["reblogs_count"] = status.ReblogsCount.ToString(),
                ["favourites_count"] = status.FavouritesCount.ToString(),
                ["author_id"] = status.Account?.Id ?? string.Empty,
                ["username"] = status.Account?.Username ?? string.Empty,
                ["acct"] = status.Account?.Acct ?? string.Empty,
                ["media_count"] = status.MediaAttachments.Count.ToString()
            },

            RawJson = JsonSerializer.Serialize(status)
        };
    }
}