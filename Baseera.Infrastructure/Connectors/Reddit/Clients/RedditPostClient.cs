using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;
using Baseera.Infrastructure.Options;
using Baseera.Service.Connectors.Reddit.Abstractions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Connectors.Reddit.Clients
{
    public class RedditPostClient : BaseRedditClient, IRedditPostClient
    {
        public RedditPostClient(
            HttpClient httpClient,
            IOptions<RedditOptions> options) : base(httpClient, options)
        {
        }

        public Task<PostDetailsResponse> GetPostDetailsAsync(
            PostDetailsRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var endpoint = BuildEndpoint(request);

            return GetAsync<PostDetailsResponse>(
                endpoint,
                cancellationToken);
        }

        private string BuildEndpoint(PostDetailsRequest request)
        {
            var queryParameters = new Dictionary<string, string?>
            {
                ["post_id"] = request.PostId
            };

            return QueryHelpers.AddQueryString(
                Options.SearchEndpoint,
                queryParameters);
        }
    }
}
