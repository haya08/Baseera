using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;

namespace Baseera.Infrastructure.Connectors.Reddit.Abstractions;

public interface IRedditPostClient
{
    Task<PostDetailsResponse> GetPostDetailsAsync(
        string url,
        CancellationToken cancellationToken = default);
}