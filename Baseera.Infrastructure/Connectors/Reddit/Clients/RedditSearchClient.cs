using Baseera.Infrastructure.Options;
using Baseera.Service.Connectors.Reddit.Abstractions;
using Baseera.Service.Connectors.Reddit.Models.Search;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Connectors.Reddit.Clients
{
    public sealed class RedditSearchClient : BaseRedditClient, IRedditSearchClient
    {
        public RedditSearchClient(
            HttpClient httpClient,
            IOptions<RedditOptions> options) : base(httpClient, options)
        {
        }

        public async Task<SearchPostsResponse> SearchAsync(
            SearchPostsRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var endpoint = BuildEndpoint(request);

            return await GetAsync<SearchPostsResponse>(
                endpoint,
                cancellationToken);
        }

        private string BuildEndpoint(SearchPostsRequest request)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["query"] = request.Query,
                ["limit"] = request.Limit.ToString(),
                ["sort"] = request.Sort,
                ["time"] = request.TimeFilter,
                ["cursor"] = request.Cursor
            };

            return QueryHelpers.AddQueryString(
                Options.SearchEndpoint,
                queryParams);
        }
    }
}
