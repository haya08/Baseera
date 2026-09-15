using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Facebook.Abstractions;
using Baseera.Infrastructure.Connectors.Facebook.Clients;
using Baseera.Infrastructure.Connectors.Facebook.ApiModels;

namespace Baseera.Infrastructure.Connectors.Facebook;

public sealed class FacebookConnector : IConnector
{
    private readonly FacebookApiClient _apiClient;
    private readonly IFacebookMapper _mapper;

    public FacebookConnector(
        FacebookApiClient apiClient,
        IFacebookMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public Platform Platform => Platform.Facebook;

    public async Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var pages = await _apiClient.GetManagedPagesAsync(
            cancellationToken);

        var page = FindMatchingPage(
            url,
            pages.Data);

        var documents = new List<UnifiedRawDocument>();

        string? after = null;

        while (true)
        {
            var response = await _apiClient.GetPostsAsync(
                page.Id,
                page.AccessToken,
                after,
                cancellationToken);

            documents.AddRange(
                response.Data.Select(_mapper.Map));

            after = response.Paging?.Cursors?.After;

            if (string.IsNullOrWhiteSpace(after))
            {
                break;
            }
        }

        return documents;
    }

    private static string ExtractPageUsername(string url)
    {
        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var uri))
        {
            throw new ArgumentException(
                "Invalid Facebook URL.",
                nameof(url));
        }

        if (!uri.Host.Equals(
                "facebook.com",
                StringComparison.OrdinalIgnoreCase) &&
            !uri.Host.Equals(
                "www.facebook.com",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "URL must be a Facebook Page URL.",
                nameof(url));
        }

        var segments = uri.AbsolutePath
            .Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 1)
        {
            throw new ArgumentException(
                "URL must point directly to a Facebook Page.",
                nameof(url));
        }

        return segments[0];
    }

    private static FacebookManagedPage FindMatchingPage(
        string inputUrl,
        IReadOnlyList<FacebookManagedPage> pages)
    {
        if (!Uri.TryCreate(
                inputUrl,
                UriKind.Absolute,
                out var inputUri))
        {
            throw new ArgumentException(
                "Invalid Facebook URL.",
                nameof(inputUrl));
        }

        var inputPath = inputUri.AbsolutePath
            .Trim('/')
            .ToLowerInvariant();

        // 1. Try matching by page link
        var pageByLink = pages.FirstOrDefault(page =>
            !string.IsNullOrWhiteSpace(page.Link) &&
            Uri.TryCreate(
                page.Link,
                UriKind.Absolute,
                out var pageUri) &&
            pageUri.AbsolutePath
                .Trim('/')
                .Equals(
                    inputPath,
                    StringComparison.OrdinalIgnoreCase));

        if (pageByLink is not null)
        {
            return pageByLink;
        }

        // 2. Fallback: match by page name
        var pageByName = pages.FirstOrDefault(page =>
            !string.IsNullOrWhiteSpace(page.Name) &&
            page.Name.Equals(
                inputPath,
                StringComparison.OrdinalIgnoreCase));

        if (pageByName is not null)
        {
            return pageByName;
        }

        throw new InvalidOperationException(
            $"No managed Facebook Page matches the URL '{inputUrl}'.");
    }

    private static string NormalizeFacebookUrl(Uri uri)
    {
        var host = uri.Host
            .Replace(
                "www.",
                string.Empty,
                StringComparison.OrdinalIgnoreCase)
            .ToLowerInvariant();

        var path = uri.AbsolutePath
            .Trim('/')
            .ToLowerInvariant();

        return $"{host}/{path}";
    }
}