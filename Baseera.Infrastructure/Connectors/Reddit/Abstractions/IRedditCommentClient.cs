using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;

namespace Baseera.Infrastructure.Connectors.Reddit.Abstractions;

public interface IRedditCommentClient
{
    Task<PostCommentResponse> GetCommentsAsync(
        string url,
        CancellationToken cancellationToken = default);
}