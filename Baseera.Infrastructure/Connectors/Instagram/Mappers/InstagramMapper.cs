using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Instagram.ApiModels;
using System.Globalization;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Instagram.Mappers;

public sealed class InstagramMapper
{
    public UnifiedRawDocument Map(
        InstagramMedia media,
        IReadOnlyList<InstagramComment> comments)
    {
        return new UnifiedRawDocument
        {
            ExternalId = media.Id,

            Platform = Platform.Instagram,

            Title = null,

            Body = media.Caption,

            Comments = comments
                .Select(MapComment)
                .ToList(),

            Author = null,

            Url = media.Permalink,

            PublishedAt = ParseInstagramDate(media.Timestamp),

            CollectedAt = DateTimeOffset.UtcNow,

            Metadata = BuildMetadata(media),

            RawJson = JsonSerializer.Serialize(
                new
                {
                    Media = media,
                    Comments = comments
                })
        };
    }

    private static UnifiedComment MapComment(
        InstagramComment comment)
    {
        return new UnifiedComment
        {
            ExternalId = comment.Id,
            Author = comment.Username,
            Body = comment.Text,
            PublishedAt = ParseInstagramDate(comment.Timestamp),
            Score = 0,
            Depth = 0
        };
    }

    private static Dictionary<string, string> BuildMetadata(
        InstagramMedia media)
    {
        var metadata = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(media.MediaType))
            metadata["media_type"] = media.MediaType;

        if (!string.IsNullOrWhiteSpace(media.MediaUrl))
            metadata["media_url"] = media.MediaUrl;

        if (!string.IsNullOrWhiteSpace(media.ThumbnailUrl))
            metadata["thumbnail_url"] = media.ThumbnailUrl;

        if (media.LikeCount.HasValue)
            metadata["like_count"] =
                media.LikeCount.Value.ToString();

        if (media.CommentsCount.HasValue)
            metadata["comments_count"] =
                media.CommentsCount.Value.ToString();

        return metadata;
    }

    private static DateTimeOffset ParseInstagramDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("Instagram timestamp is missing.");

        // Instagram may return: 2026-09-16T20:19:20+0000
        // DateTimeOffset expects: 2026-09-16T20:19:20+00:00

        if (value.Length >= 5)
        {
            var signIndex = value.Length - 5;

            if ((value[signIndex] == '+' || value[signIndex] == '-') &&
                value[^3] != ':')
            {
                value = value.Insert(value.Length - 2, ":");
            }
        }

        return DateTimeOffset.Parse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal |
            DateTimeStyles.AdjustToUniversal);
    }
}