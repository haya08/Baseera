using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Mastodon.ApiModels;
using Baseera.Infrastructure.Connectors.Mastodon.Mappers;
using System.Text.Json;

namespace Baseera.Infrastructure.Connectors.Mastodon;

public sealed class MastodonConnector : IConnector
{
    private readonly HttpClient _httpClient;
    private readonly IMastodonMapper _mapper;

    public MastodonConnector(
        HttpClient httpClient,
        IMastodonMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public Platform Platform => Platform.Mastodon;

    public async Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var instanceUri))
        {
            throw new ArgumentException(
                "Invalid Mastodon instance URL.",
                nameof(url));
        }

        var endpoint =
            $"{instanceUri.GetLeftPart(UriPartial.Authority)}" +
            "/api/v1/timelines/public?limit=10&local=true";

        using var response = await _httpClient.GetAsync(
            endpoint,
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Mastodon public timeline request failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {json}");
        }

        var statuses =
            JsonSerializer.Deserialize<List<MastodonStatus>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (statuses is null)
        {
            return [];
        }

        return statuses
            .Select(_mapper.Map)
            .ToList();
    }
}