namespace Baseera.Infrastructure.Connectors.Facebook;

public sealed class FacebookApiOptions
{
    public string BaseUrl { get; init; } =
        "https://graph.facebook.com";

    public string ApiVersion { get; init; } =
        "v26.0";

    public string UserAccessToken { get; init; } =
        string.Empty;
}