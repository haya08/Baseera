using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Facebook.ApiModels;

namespace Baseera.Infrastructure.Connectors.Facebook.Abstractions;

public interface IFacebookMapper
{
    UnifiedRawDocument Map(
        FacebookPost post);
}