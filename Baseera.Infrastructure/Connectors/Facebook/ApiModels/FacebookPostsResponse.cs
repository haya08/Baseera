namespace Baseera.Infrastructure.Connectors.Facebook.ApiModels;

public sealed class FacebookPostsResponse
{
    public IReadOnlyList<FacebookPost> Data { get; init; } = [];

    public FacebookPaging? Paging { get; init; }
}

public sealed class FacebookPaging
{
    public string? Next { get; init; }

    public string? Previous { get; init; }

    public FacebookPagingCursors? Cursors { get; init; }
}

public sealed class FacebookPagingCursors
{
    public string? Before { get; init; }

    public string? After { get; init; }
}