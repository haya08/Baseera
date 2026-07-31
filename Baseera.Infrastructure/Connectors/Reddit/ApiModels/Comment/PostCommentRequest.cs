namespace Baseera.Infrastructure.Connectors.Reddit.ApiModels.Comment
{
    public sealed class PostCommentsRequest
    {
        public required string PostId { get; init; }
    }
}
