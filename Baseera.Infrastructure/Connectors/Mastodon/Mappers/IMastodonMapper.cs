using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Mastodon.ApiModels;

namespace Baseera.Infrastructure.Connectors.Mastodon.Mappers;

public interface IMastodonMapper
{
    UnifiedRawDocument Map(MastodonStatus status);
}