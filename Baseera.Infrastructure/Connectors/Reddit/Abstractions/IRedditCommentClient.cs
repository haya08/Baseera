using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;

namespace Baseera.Service.Connectors.Reddit.Abstractions
{
    public interface IRedditCommentClient
    {
        Task<PostCommentResponse> GetCommentsAsync(
            PostCommentsRequest request,
            CancellationToken cancellationToken = default);
    }
}
