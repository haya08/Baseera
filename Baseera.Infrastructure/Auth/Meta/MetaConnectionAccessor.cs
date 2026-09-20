using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Baseera.Infrastructure.Auth.Meta;

public interface IMetaConnectionAccessor
{
    MetaOAuthResult GetConnection();

    MetaPageConnection GetInstagramConnection();
}

public sealed class MetaConnectionAccessor : IMetaConnectionAccessor
{
    private const string SessionKey = "MetaConnection";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public MetaConnectionAccessor(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public MetaOAuthResult GetConnection()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException(
                "No active HTTP context.");

        var json = httpContext.Session.GetString(SessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException(
                "No Meta connection found. Please connect Meta first.");
        }

        var connection =
            JsonSerializer.Deserialize<MetaOAuthResult>(json);

        return connection
            ?? throw new InvalidOperationException(
                "Meta connection could not be deserialized.");
    }

    public MetaPageConnection GetInstagramConnection()
    {
        var connection = GetConnection();

        var page = connection.Pages.FirstOrDefault(
            p => !string.IsNullOrWhiteSpace(
                p.InstagramBusinessAccountId));

        return page
            ?? throw new InvalidOperationException(
                "No connected Instagram Business Account was found.");
    }
}