using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment;
using Baseera.Infrastructure.Connectors.Reddit.ApiModels.Post;

namespace Baseera.Infrastructure.Connectors.Reddit.Abstractions
{
    public interface IRedditMapper
    {
        UnifiedRawDocument Map(
            PostDetailsResponse post,
            PostCommentResponse comments);
    }
}
