using Baseera.Service.Connectors.Reddit.Abstractions;
using Baseera.Service.Connectors.Reddit.Models;

namespace Baseera.Service.Connectors.Reddit.Implementations
{
    public sealed class RedditClient : IRedditClient
    {
        private readonly HttpClient _httpClient;

        public RedditClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RedditSearchResponse> SearchAsync(RedditSearchRequest request, CancellationToken cancellationToken = default)
        {
            // validate request
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                throw new ArgumentException(
                    "Query cannot be empty.",
                    nameof(request.Query));
            }

            // build request url
            var query = Uri.EscapeDataString(request.Query);
            var url =
            $"/search.json?q={query}" +
            $"&limit={request.Limit}" +
            $"&sort={request.Sort}" +
            $"&t={request.Time}";

            // pagination
            if (!string.IsNullOrWhiteSpace(request.After))
            {
                url += $"&after={request.After}";
            }

            // send reddit request
            var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            // receive and deserialize response
            var stream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            var result =
                await System.Text.Json.JsonSerializer.DeserializeAsync<RedditSearchResponse>(
                    stream,
                    cancellationToken: cancellationToken);

            if (result is null)
            {
                throw new InvalidOperationException(
                    "Reddit returned an empty response.");
            }

            return result;
        }
    }
}
