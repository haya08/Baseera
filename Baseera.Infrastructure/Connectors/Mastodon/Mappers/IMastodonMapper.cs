using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Infrastructure.Connectors.Mastodon.ApiModels;

namespace Baseera.Infrastructure.Connectors.Mastodon.Mappers;

public interface IMastodonMapper
{
    UnifiedRawDocument Map(MastodonStatus status);
}