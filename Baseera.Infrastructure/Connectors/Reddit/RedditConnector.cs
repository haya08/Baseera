using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;


namespace Baseera.Infrastructure.Connectors.Reddit;

public sealed class RedditConnector : IConnector
{
    private readonly IRedditPostClient _postClient;
    private readonly IRedditCommentClient _commentClient;
    private readonly IRedditMapper _mapper;

    public RedditConnector(
        IRedditPostClient postClient,
        IRedditCommentClient commentClient,
        IRedditMapper mapper)
    {
        _postClient = postClient;
        _commentClient = commentClient;
        _mapper = mapper;
    }

    public Platform Platform => Platform.Reddit;

    public async Task<IReadOnlyList<UnifiedRawDocument>> GetDocumentsAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var post = await _postClient.GetPostDetailsAsync(
            url,
            cancellationToken);

        var comments = await _commentClient.GetCommentsAsync(
            url,
            cancellationToken);

        var document = _mapper.Map(
            post,
            comments);

        return [document];
    }
}