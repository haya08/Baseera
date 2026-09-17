using Baseera.Infrastructure.Connectors.Instagram.ApiModels;

namespace Baseera.Infrastructure.Connectors.Instagram.Abstractions;

public interface IInstagramApiClient
{
    Task<IReadOnlyList<InstagramMedia>> GetAllMediaAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InstagramComment>> GetAllCommentsAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}