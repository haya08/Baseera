using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;

namespace Baseera.Service.Connectors.Reddit.Abstractions
{
    public interface IRedditPostClient
    {
        Task<PostDetailsResponse> GetPostDetailsAsync(
            PostDetailsRequest request,
            CancellationToken cancellationToken = default);
    }
}
