using Baseera.Domain.Entities;
using Baseera.Domain.Enums;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Documents;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;
using Baseera.Service.Connectors.Reddit.Abstractions;
using Baseera.Service.Connectors.Reddit.Models.Search;

namespace Baseera.Infrastructure.Connectors.Reddit
{
    public sealed class RedditConnector : IConnector
    {
        private readonly IRedditSearchClient _searchClient;
        private readonly IRedditPostClient _postClient;
        private readonly IRedditCommentClient _commentClient;
        private readonly IRedditMapper _mapper;

        public RedditConnector(
            IRedditSearchClient searchClient,
            IRedditPostClient postClient,
            IRedditCommentClient commentClient,
            IRedditMapper mapper)
        {
            _searchClient = searchClient;
            _postClient = postClient;
            _commentClient = commentClient;
            _mapper = mapper;
        }

        public Platform Platform => Platform.Reddit;

        public async Task<IReadOnlyList<UnifiedRawDocument>> CollectAsync(
            TbSearchQuery query,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);

            var documents = new List<UnifiedRawDocument>();

            var searchResponse = await _searchClient.SearchAsync(
                new SearchPostsRequest
                {
                    Query = query.QueryText
                },
                cancellationToken);

            if (searchResponse?.Posts is null ||
                searchResponse.Posts.Count == 0)
            {
                return documents;
            }

            foreach (var searchPost in searchResponse.Posts)
            {
                var postDetails = await _postClient.GetPostDetailsAsync(
                    searchPost.PostId,
                    cancellationToken);

                var comments = await _commentClient.GetCommentsAsync(
                    searchPost.PostId,
                    cancellationToken);

                var document = _mapper.Map(
                    postDetails,
                    comments);

                documents.Add(document);
            }

            return documents;
        }
    }
}
