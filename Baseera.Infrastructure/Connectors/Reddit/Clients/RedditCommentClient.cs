using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;
using Baseera.Infrastructure.Options;
using Baseera.Service.Connectors.Reddit.Abstractions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Connectors.Reddit.Clients
{
    public class RedditCommentClient : BaseRedditClient, IRedditCommentClient
    {
        public RedditCommentClient(
            HttpClient httpClient,
            IOptions<RedditOptions> options) : base(httpClient, options)
        {
        }

        public Task<PostCommentResponse> GetCommentsAsync(
            PostCommentsRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var endpoint = BuildEndpoint(request);

            return GetAsync<PostCommentResponse>(
                endpoint,
                cancellationToken);
        }

        private string BuildEndpoint(PostCommentsRequest request)
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
