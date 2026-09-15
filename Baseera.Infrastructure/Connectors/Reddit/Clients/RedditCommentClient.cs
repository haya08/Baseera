using System.Text.Json;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;
using Microsoft.Extensions.Options;


namespace Baseera.Infrastructure.Connectors.Reddit.Clients;

public sealed class RedditCommentClient : IRedditCommentClient
{
    private readonly HttpClient _httpClient;
    private readonly RedditApiOptions _options;

    public RedditCommentClient(
        HttpClient httpClient,
        IOptions<RedditApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<PostCommentResponse> GetCommentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var endpoint =
            $"/reddit/comments?post_id={Uri.EscapeDataString(url)}" +
            "&sort=best&limit=100";

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
                $"Reddit comments request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var result = JsonSerializer.Deserialize<PostCommentResponse>(
            json);

        if (result is null)
        {
            throw new InvalidOperationException(
                "Reddit comments response could not be deserialized.");
        }

        if (!result.Success)
        {
            throw new InvalidOperationException(
                "PullAPI returned an unsuccessful Reddit comments response.");
        }

        return result;
    }
}