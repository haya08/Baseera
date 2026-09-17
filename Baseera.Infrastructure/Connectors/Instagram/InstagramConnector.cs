using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Instagram.Abstractions;
using Baseera.Infrastructure.Connectors.Instagram.Mappers;

namespace Baseera.Infrastructure.Connectors.Instagram;

public sealed class InstagramConnector : IConnector
{
    private readonly IInstagramApiClient _apiClient;
    private readonly InstagramMapper _mapper;

    public InstagramConnector(
        IInstagramApiClient apiClient,
        InstagramMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    public Platform Platform => Platform.Instagram;

    public async Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var media = await _apiClient.GetAllMediaAsync(
            cancellationToken);

        var documents = new List<UnifiedRawDocument>();

        foreach (var item in media)
        {
            var comments =
                await _apiClient.GetAllCommentsAsync(
                    item.Id,
                    cancellationToken);

            documents.Add(
                _mapper.Map(item, comments));
        }

        return documents;
    }
}