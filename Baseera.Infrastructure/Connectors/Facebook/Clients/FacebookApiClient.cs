using System.Text.Json;
using Baseera.Infrastructure.Connectors.Facebook.ApiModels;
using Microsoft.Extensions.Options;
using Baseera.Infrastructure.Auth.Meta;

namespace Baseera.Infrastructure.Connectors.Facebook.Clients;

public sealed class FacebookApiClient
{
    private readonly HttpClient _httpClient;
    private readonly FacebookApiOptions _options;
    private readonly IMetaConnectionAccessor _connectionAccessor;

    public FacebookApiClient(
        HttpClient httpClient,
        IOptions<FacebookApiOptions> options,
        IMetaConnectionAccessor connectionAccessor)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _connectionAccessor = connectionAccessor;
    }

    public async Task<FacebookManagedPagesResponse> GetManagedPagesAsync(
        CancellationToken cancellationToken = default)
    {

        var connection = _connectionAccessor.GetConnection();

        var endpoint =
        $"/{_options.ApiVersion}/me/accounts" +
        "?fields=id,name,link,access_token" +
        $"&access_token={Uri.EscapeDataString(connection.UserAccessToken)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            endpoint);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Facebook managed pages request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var result =
            JsonSerializer.Deserialize<FacebookManagedPagesResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result is null)
        {
            throw new InvalidOperationException(
                "Facebook managed pages response could not be deserialized.");
        }

        return result;
    }

    public async Task<FacebookPostsResponse> GetPostsAsync(
        string pageId,
        string pageAccessToken,
        string? after = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(pageAccessToken);

        var endpoint =
            $"/{_options.ApiVersion}/{pageId}/posts" +
            "?fields=id,message,created_time,permalink_url,full_picture,attachments,shares" +
            $"&access_token={Uri.EscapeDataString(pageAccessToken)}";

        if (!string.IsNullOrWhiteSpace(after))
        {
            endpoint +=
                $"&after={Uri.EscapeDataString(after)}";
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            endpoint);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Facebook posts request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var result =
            JsonSerializer.Deserialize<FacebookPostsResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result is null)
        {
            throw new InvalidOperationException(
                "Facebook posts response could not be deserialized.");
        }

        return result;
    }
}