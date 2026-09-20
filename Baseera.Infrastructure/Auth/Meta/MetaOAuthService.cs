using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure.Auth.Meta;

public sealed class MetaOAuthService : IMetaOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly MetaOAuthOptions _options;

    public MetaOAuthService(
        HttpClient httpClient,
        IOptions<MetaOAuthOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public string BuildAuthorizationUrl(string state)
    {
        var query =
            $"client_id={Uri.EscapeDataString(_options.AppId)}" +
            $"&config_id={Uri.EscapeDataString(_options.ConfigId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            $"&response_type=code";

        return $"https://www.facebook.com/{_options.GraphVersion}/dialog/oauth?{query}";
    }

    public async Task<MetaOAuthResult> CompleteAuthorizationAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var tokenUrl =
            $"https://graph.facebook.com/{_options.GraphVersion}/oauth/access_token" +
            $"?client_id={Uri.EscapeDataString(_options.AppId)}" +
            $"&client_secret={Uri.EscapeDataString(_options.AppSecret)}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&code={Uri.EscapeDataString(code)}";

        using var tokenResponse = await _httpClient.GetAsync(
            tokenUrl,
            cancellationToken);

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync(
            cancellationToken);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Meta token exchange failed. " +
                $"Status: {(int)tokenResponse.StatusCode}. " +
                $"Response: {tokenJson}");
        }

        using var tokenDocument =
            JsonDocument.Parse(tokenJson);

        var userAccessToken =
            tokenDocument.RootElement
                .GetProperty("access_token")
                .GetString()
            ?? throw new InvalidOperationException(
                "Meta did not return an access token.");

        var pagesUrl =
            $"https://graph.facebook.com/{_options.GraphVersion}" +
            $"/me/accounts" +
            $"?fields=id,name,access_token,tasks,instagram_business_account" +
            $"&access_token={Uri.EscapeDataString(userAccessToken)}";

        using var pagesResponse = await _httpClient.GetAsync(
            pagesUrl,
            cancellationToken);

        var pagesJson = await pagesResponse.Content.ReadAsStringAsync(
            cancellationToken);

        if (!pagesResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Meta pages request failed. " +
                $"Status: {(int)pagesResponse.StatusCode}. " +
                $"Response: {pagesJson}");
        }

        using var pagesDocument =
            JsonDocument.Parse(pagesJson);

        var pages = new List<MetaPageConnection>();

        if (pagesDocument.RootElement.TryGetProperty("data", out var data))
        {
            foreach (var page in data.EnumerateArray())
            {
                var pageId =
                    page.GetProperty("id").GetString() ?? string.Empty;

                var pageName =
                    page.GetProperty("name").GetString() ?? string.Empty;

                var pageAccessToken =
                    page.GetProperty("access_token").GetString()
                    ?? string.Empty;

                string? instagramBusinessAccountId = null;

                if (page.TryGetProperty(
                        "instagram_business_account",
                        out var instagramAccount))
                {
                    instagramBusinessAccountId =
                        instagramAccount
                            .GetProperty("id")
                            .GetString();
                }

                pages.Add(new MetaPageConnection
                {
                    PageId = pageId,
                    PageName = pageName,
                    PageAccessToken = pageAccessToken,
                    InstagramBusinessAccountId =
                        instagramBusinessAccountId
                });
            }
        }

        return new MetaOAuthResult
        {
            UserAccessToken = userAccessToken,
            Pages = pages
        };
    }
}