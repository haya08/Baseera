using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Facebook.Abstractions;
using Baseera.Infrastructure.Connectors.Facebook.ApiModels;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Facebook.Mappers;

public sealed class FacebookMapper : IFacebookMapper
{
    public UnifiedRawDocument Map(FacebookPost post)
    {
        ArgumentNullException.ThrowIfNull(post);

        return new UnifiedRawDocument
        {
            ExternalId = post.Id,

            Platform = Platform.Facebook,

            Title = null,

            Body = post.Message,

            Comments = [],

            Author = null,

            Url = post.PermalinkUrl,

            PublishedAt = ParseFacebookDate(post.CreatedTime),

            CollectedAt = DateTimeOffset.UtcNow,

            Metadata = BuildMetadata(post),

            RawJson = JsonSerializer.Serialize(post)
        };
    }


    private static IReadOnlyDictionary<string, string> BuildMetadata(
        FacebookPost post)
    {
        var metadata = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(post.FullPicture))
        {
            metadata["full_picture"] = post.FullPicture;
        }

        if (post.Shares is not null)
        {
            metadata["share_count"] =
                post.Shares.Count.ToString();
        }

        if (post.Attachments?.Data is not { Count: > 0 } attachments)
        {
            return metadata;
        }

        metadata["attachment_count"] =
            attachments.Count.ToString();

        for (var i = 0; i < attachments.Count; i++)
        {
            var attachment = attachments[i];

            var prefix = $"attachment_{i}";

            // General attachment information
            if (!string.IsNullOrWhiteSpace(attachment.Type))
            {
                metadata[$"{prefix}_type"] =
                    attachment.Type;
            }

            if (!string.IsNullOrWhiteSpace(attachment.Description))
            {
                metadata[$"{prefix}_description"] =
                    attachment.Description;
            }

            if (!string.IsNullOrWhiteSpace(attachment.Url))
            {
                metadata[$"{prefix}_url"] =
                    attachment.Url;
            }

            if (!string.IsNullOrWhiteSpace(
                    attachment.Target?.Url))
            {
                metadata[$"{prefix}_target_url"] =
                    attachment.Target.Url;
            }

            if (!string.IsNullOrWhiteSpace(
                    attachment.Target?.Id))
            {
                metadata[$"{prefix}_target_id"] =
                    attachment.Target.Id;
            }

            var attachmentType =
                attachment.Type?.ToLowerInvariant();


            if (attachmentType is "video" or "video_inline")
            {
                metadata[$"{prefix}_media_type"] = "video";

                if (!string.IsNullOrWhiteSpace(
                        attachment.Media?.Video?.Source))
                {
                    metadata[$"{prefix}_video_url"] =
                        attachment.Media.Video.Source;
                }
                else if (!string.IsNullOrWhiteSpace(
                            attachment.Url))
                {
                    metadata[$"{prefix}_video_url"] =
                        attachment.Url;
                }

                if (!string.IsNullOrWhiteSpace(
                        attachment.Media?.Image?.Src))
                {
                    metadata[$"{prefix}_thumbnail_url"] =
                        attachment.Media.Image.Src;
                }

                if (attachment.Media?.Image?.Width is not null)
                {
                    metadata[$"{prefix}_thumbnail_width"] =
                        attachment.Media.Image.Width.Value.ToString();
                }

                if (attachment.Media?.Image?.Height is not null)
                {
                    metadata[$"{prefix}_thumbnail_height"] =
                        attachment.Media.Image.Height.Value.ToString();
                }

                if (attachment.Media?.Video?.Width is not null)
                {
                    metadata[$"{prefix}_video_width"] =
                        attachment.Media.Video.Width.Value.ToString();
                }

                if (attachment.Media?.Video?.Height is not null)
                {
                    metadata[$"{prefix}_video_height"] =
                        attachment.Media.Video.Height.Value.ToString();
                }
            }


            else if (!string.IsNullOrWhiteSpace(
                        attachment.Media?.Image?.Src))
            {
                metadata[$"{prefix}_media_type"] = "image";

                metadata[$"{prefix}_image_url"] =
                    attachment.Media.Image.Src;

                if (attachment.Media.Image.Width is not null)
                {
                    metadata[$"{prefix}_image_width"] =
                        attachment.Media.Image.Width.Value.ToString();
                }

                if (attachment.Media.Image.Height is not null)
                {
                    metadata[$"{prefix}_image_height"] =
                        attachment.Media.Image.Height.Value.ToString();
                }
            }
        }

        return metadata;
    }


    private static DateTimeOffset ParseFacebookDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateTimeOffset.UtcNow;

        if (value.Length >= 5)
        {
            var offsetStart = value.Length - 5;

            if (value[offsetStart] is '+' or '-')
            {
                value = value.Insert(
                    value.Length - 2,
                    ":");
            }
        }

        if (DateTimeOffset.TryParse(
            value,
            out var result))
        {
            return result;
        }

        throw new FormatException(
            $"Invalid Facebook created_time value: {value}");
    }
}