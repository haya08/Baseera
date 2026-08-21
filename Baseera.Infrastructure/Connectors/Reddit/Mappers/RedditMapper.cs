using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Reddit.Mappers
{
    public sealed class RedditMapper : IRedditMapper
    {
        public UnifiedRawDocument Map(
            PostDetailsResponse post,
            PostCommentResponse comments)
        {
            return new UnifiedRawDocument
            {
                ExternalId = post.Data.PostId,

                Platform = Platform.Reddit,

                Url = post.Data.Permalink,

                Title = post.Data.Title,

                Body = post.Data.SelfText,

                Author = post.Data.Author,

                PublishedAt = post.Data.CreatedUtc,

                Metadata = CreateMetadata(post),

                Comments = MapComments(comments),

                //Attachments = MapAttachments(post),

                RawJson = JsonSerializer.Serialize(
                    new
                    {
                        Post = post,
                        Comments = comments
                    })
            };
        }

        private static IReadOnlyList<UnifiedComment> MapComments(
            PostCommentResponse response)
        {
            return response.Data.Comments
                .Select(comment => new UnifiedComment
                {
                    ExternalId = comment.CommentId,

                    Author = comment.Author,

                    Body = comment.Body,

                    PublishedAt = comment.CreatedUtc,

                    Score = comment.Score,

                    Depth = comment.Depth
                })
                .ToList();
        }

        //private static IReadOnlyList<UnifiedAttachment> MapAttachments(
        //    PostDetailsApiResponse post)
        //{
        //    var attachments = new List<UnifiedAttachment>();

        //    if (!string.IsNullOrWhiteSpace(post.Data.MediaUrl))
        //    {
        //        attachments.Add(new UnifiedAttachment
        //        {
        //            Type = post.Data.IsVideo
        //                ? AttachmentType.Video
        //                : AttachmentType.Image,

        //            Url = post.Data.MediaUrl,

        //            ThumbnailUrl = post.Data.Thumbnail
        //        });
        //    }

        //    return attachments;
        //}

        private static Dictionary<string, string> CreateMetadata(
            PostDetailsResponse post)
        {
            return new Dictionary<string, string>
            {
                ["score"] = post.Data.Score.ToString(),
                ["upvoteRatio"] = post.Data.UpvoteRatio.ToString(),
                ["numComments"] = post.Data.NumComments.ToString(),
                ["awardsCount"] = post.Data.AwardsCount.ToString(),
                ["subreddit"] = post.Data.Subreddit,
                ["domain"] = post.Data.Domain,
                ["isNsfw"] = post.Data.IsNsfw.ToString(),
                ["isSpoiler"] = post.Data.IsSpoiler.ToString(),
                ["isSelf"] = post.Data.IsSelf.ToString(),
                ["isVideo"] = post.Data.IsVideo.ToString(),
                ["linkFlair"] = post.Data.LinkFlairText
            };
        }
    }
}
