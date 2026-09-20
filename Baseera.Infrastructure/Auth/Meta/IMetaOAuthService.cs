namespace Baseera.Infrastructure.Auth.Meta;

public interface IMetaOAuthService
{
    string BuildAuthorizationUrl(string state);

    Task<MetaOAuthResult> CompleteAuthorizationAsync(
        string code,
        CancellationToken cancellationToken = default);
}

public sealed class MetaOAuthResult
{
    public string UserAccessToken { get; init; } = string.Empty;

    public List<MetaPageConnection> Pages { get; init; } = [];
}

public sealed class MetaPageConnection
{
    public string PageId { get; init; } = string.Empty;
    public string PageName { get; init; } = string.Empty;
    public string PageAccessToken { get; init; } = string.Empty;
    public string? InstagramBusinessAccountId { get; init; }
}