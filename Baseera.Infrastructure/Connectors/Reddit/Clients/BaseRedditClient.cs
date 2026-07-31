using Baseera.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Reddit.Clients
{
    public abstract class BaseRedditClient
    {
        protected readonly HttpClient HttpClient;
        protected readonly RedditOptions Options;

        protected BaseRedditClient(
            HttpClient httpClient,
            IOptions<RedditOptions> options)
        {
            HttpClient = httpClient;
            Options = options.Value;
        }

        protected async Task<T> GetAsync<T>(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync(
                endpoint,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            var result = await JsonSerializer.DeserializeAsync<T>(
                stream,
                cancellationToken: cancellationToken);

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize response to {typeof(T).Name}.");
            }

            return result;
        }
    }
}
