using System.Net;
using System.Text.Json;
using Baseera.Infrastructure.Connectors.Instagram.Abstractions;
using Baseera.Infrastructure.Connectors.Instagram.ApiModels;
using Microsoft.Extensions.Options;
using Baseera.Infrastructure.Auth.Meta;

namespace Baseera.Infrastructure.Connectors.Instagram.Clients;

public sealed class InstagramApiClient : IInstagramApiClient
{
    private readonly HttpClient _httpClient;
    private readonly InstagramApiOptions _options;
    private readonly IMetaConnectionAccessor _connectionAccessor;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public InstagramApiClient(
    HttpClient httpClient,
    IOptions<InstagramApiOptions> options,
    IMetaConnectionAccessor connectionAccessor)
    {
    _httpClient = httpClient;
    _options = options.Value;
    _connectionAccessor = connectionAccessor;
    }

    public async Task<IReadOnlyList<InstagramMedia>> GetAllMediaAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = _connectionAccessor.GetInstagramConnection();

        ArgumentException.ThrowIfNullOrWhiteSpace(
            connection.InstagramBusinessAccountId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            connection.PageAccessToken);

        var fields =
            "id,caption,media_type,media_url,permalink,timestamp,thumbnail_url,like_count,comments_count";

        var url =
            $"{_options.ApiVersion}/" +
            $"{connection.InstagramBusinessAccountId}/media" +
            $"?fields={Uri.EscapeDataString(fields)}" +
            $"&access_token={Uri.EscapeDataString(connection.PageAccessToken)}";

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

        var connection = _connectionAccessor.GetInstagramConnection();

        ArgumentException.ThrowIfNullOrWhiteSpace(
            connection.PageAccessToken);

        var fields = "id,text,username,timestamp";

        var url =
            $"{_options.ApiVersion}/{mediaId}/comments" +
            $"?fields={Uri.EscapeDataString(fields)}" +
            $"&access_token={Uri.EscapeDataString(connection.PageAccessToken)}";

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