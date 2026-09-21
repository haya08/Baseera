using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;

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