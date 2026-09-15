using System.Text.Json;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Connectors.Reddit.Clients;

public sealed class RedditPostClient : IRedditPostClient
{
    private readonly HttpClient _httpClient;
    private readonly RedditApiOptions _options;

    public RedditPostClient(
        HttpClient httpClient,
        IOptions<RedditApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<PostDetailsResponse> GetPostDetailsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var endpoint =
            $"/reddit/post?post_id={Uri.EscapeDataString(url)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            endpoint);

        request.Headers.TryAddWithoutValidation(
            "x-rapidapi-key",
            _options.ApiKey);

        request.Headers.TryAddWithoutValidation(
            "x-rapidapi-host",
            _options.ApiHost);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Reddit post request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var result = JsonSerializer.Deserialize<PostDetailsResponse>(
            json);

        if (result is null)
        {
            throw new InvalidOperationException(
                "Reddit post response could not be deserialized.");
        }

        if (!result.Success)
        {
            throw new InvalidOperationException(
                "PullAPI returned an unsuccessful Reddit post response.");
        }

        return result;
    }
}