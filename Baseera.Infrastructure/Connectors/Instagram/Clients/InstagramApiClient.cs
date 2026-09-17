using System.Net;
using System.Text.Json;
using Baseera.Infrastructure.Connectors.Instagram.Abstractions;
using Baseera.Infrastructure.Connectors.Instagram.ApiModels;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Connectors.Instagram.Clients;

public sealed class InstagramApiClient : IInstagramApiClient
{
    private readonly HttpClient _httpClient;
    private readonly InstagramApiOptions _options;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public InstagramApiClient(
        HttpClient httpClient,
        IOptions<InstagramApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<InstagramMedia>> GetAllMediaAsync(
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            _options.InstagramBusinessAccountId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            _options.PageAccessToken);

        var fields =
            "id,caption,media_type,media_url,permalink,timestamp,thumbnail_url,like_count,comments_count";

        var url =
            $"{_options.ApiVersion}/" +
            $"{_options.InstagramBusinessAccountId}/media" +
            $"?fields={Uri.EscapeDataString(fields)}" +
            $"&access_token={Uri.EscapeDataString(_options.PageAccessToken)}";

        var result = new List<InstagramMedia>();

        while (!string.IsNullOrWhiteSpace(url))
        {
            using var response = await _httpClient.GetAsync(
                url,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Instagram media request failed. " +
                    $"Status: {(int)response.StatusCode}. " +
                    $"Response: {body}");
            }

            var page =
                JsonSerializer.Deserialize<InstagramMediaResponse>(
                    body,
                    JsonOptions)
                ?? throw new JsonException(
                    "Instagram media response was empty.");

            result.AddRange(page.Data);

            url = page.Paging?.Next ?? string.Empty;
        }

        return result;
    }

    public async Task<IReadOnlyList<InstagramComment>> GetAllCommentsAsync(
        string mediaId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            _options.PageAccessToken);

        var fields = "id,text,username,timestamp";

        var url =
            $"{_options.ApiVersion}/{mediaId}/comments" +
            $"?fields={Uri.EscapeDataString(fields)}" +
            $"&access_token={Uri.EscapeDataString(_options.PageAccessToken)}";

        var result = new List<InstagramComment>();

        while (!string.IsNullOrWhiteSpace(url))
        {
            using var response = await _httpClient.GetAsync(
                url,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Instagram comments request failed. " +
                    $"Status: {(int)response.StatusCode}. " +
                    $"Response: {body}");
            }

            var page =
                JsonSerializer.Deserialize<InstagramCommentsResponse>(
                    body,
                    JsonOptions)
                ?? throw new JsonException(
                    "Instagram comments response was empty.");

            result.AddRange(page.Data);

            url = page.Paging?.Next ?? string.Empty;
        }

        return result;
    }
}