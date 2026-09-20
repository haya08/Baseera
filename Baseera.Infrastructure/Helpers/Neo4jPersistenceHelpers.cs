using Baseera.Domain.Entities;
using Baseera.Domain.Enums;

namespace Baseera.Infrastructure.Helpers
{
    public static class Neo4jPersistenceHelpers
    {
        public static string GetEntityLabel(BaseEntity entity)
        {
            return entity.GetType().Name;
        }

        public static string GetRelationshipType(
            RelationshipType relationshipType)
        {
            return relationshipType switch
            {
                RelationshipType.HasAlias => "HAS_ALIAS",
                RelationshipType.HasProduct => "HAS_PRODUCT",
                RelationshipType.HasService => "HAS_SERVICE",
                RelationshipType.HasFeature => "HAS_FEATURE",
                RelationshipType.HasTopic => "HAS_TOPIC",
                RelationshipType.RunsCampaign => "RUNS_CAMPAIGN",
                RelationshipType.HasEvent => "HAS_EVENT",
                RelationshipType.RelatedTo => "RELATED_TO",
                RelationshipType.LedBy => "LED_BY",

                _ => throw new ArgumentOutOfRangeException(
                    nameof(relationshipType),
                    relationshipType,
                    "Unsupported relationship type.")
            };
        }
    }
}
