using System.Text.Json;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Bluesky.ApiModels;
using Baseera.Infrastructure.Connectors.Bluesky.Mappers;

namespace Baseera.Infrastructure.Connectors.Bluesky;

public sealed class BlueskyConnector : IConnector
{
    private readonly HttpClient _httpClient;
    private readonly IBlueskyMapper _mapper;

    public BlueskyConnector(
        HttpClient httpClient,
        IBlueskyMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public Platform Platform => Platform.Bluesky;

    public async Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var handle = ExtractHandle(url);

        var endpoint =
            $"/xrpc/app.bsky.feed.getAuthorFeed" +
            $"?actor={Uri.EscapeDataString(handle)}" +
            "&limit=20";

        using var response = await _httpClient.GetAsync(
            endpoint,
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Bluesky author feed request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var result =
            JsonSerializer.Deserialize<BlueskyFeedResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result is null)
        {
            throw new InvalidOperationException(
                "Bluesky response could not be deserialized.");
        }

        return result.Feed
            .Where(x => x.Post is not null)
            .Select(_mapper.Map)
            .ToList();
    }

    private static string ExtractHandle(string url)
    {
        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var uri))
        {
            throw new ArgumentException(
                "Invalid Bluesky URL.",
                nameof(url));
        }

        if (!uri.Host.Equals(
                "bsky.app",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "URL must be a Bluesky profile URL.",
                nameof(url));
        }

        var segments = uri.AbsolutePath
            .Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

        var profileIndex = Array.FindIndex(
            segments,
            x => x.Equals(
                "profile",
                StringComparison.OrdinalIgnoreCase));

        if (profileIndex < 0 ||
            profileIndex + 1 >= segments.Length)
        {
            throw new ArgumentException(
                "Invalid Bluesky profile URL.",
                nameof(url));
        }

        return segments[profileIndex + 1];
    }
}