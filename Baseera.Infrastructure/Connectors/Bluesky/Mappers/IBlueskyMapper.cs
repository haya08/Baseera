using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Infrastructure.Connectors.Bluesky.ApiModels;

namespace Baseera.Infrastructure.Connectors.Bluesky.Mappers;

public interface IBlueskyMapper
{
    UnifiedRawDocument Map(BlueskyFeedItem item);
}