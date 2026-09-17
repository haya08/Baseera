namespace Baseera.Infrastructure.Connectors.Instagram;

public sealed class InstagramApiOptions
{
    public string BaseUrl { get; set; } = "https://graph.facebook.com";
    public string ApiVersion { get; set; } = "v26.0";
    public string InstagramBusinessAccountId { get; set; } = string.Empty;
    public string PageAccessToken { get; set; } = string.Empty;
}