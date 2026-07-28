using Baseera.Service.Connectors.Reddit.Models;

namespace Baseera.Service.Connectors.Reddit.Abstractions
{
    public interface IRedditClient
    {
        Task<RedditSearchResponse> SearchAsync(
            RedditSearchRequest request,
            CancellationToken cancellationToken = default);
    }
}
