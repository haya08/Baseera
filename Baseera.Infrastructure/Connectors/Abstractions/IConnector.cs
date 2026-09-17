using Baseera.Domain.Entities;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Documents;

namespace Baseera.Infrastructure.Connectors.Abstractions
{
    public interface IConnector
    {
    Platform Platform { get; }

    Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default);
}

}