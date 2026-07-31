using Baseera.Service.Connectors.Reddit.Models.Search;

namespace Baseera.Service.Connectors.Reddit.Abstractions
{
    public interface IRedditSearchClient
    {
        Task<SearchPostsResponse> SearchAsync(
            SearchPostsRequest request,
            CancellationToken cancellationToken = default);
    }
}
