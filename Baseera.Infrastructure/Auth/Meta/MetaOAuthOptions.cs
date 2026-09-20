namespace Baseera.Infrastructure.Auth.Meta;

public sealed class MetaOAuthOptions
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;

    // Facebook Login for Business configuration id
    public string ConfigId { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;
    public string GraphVersion { get; set; } = "v26.0";
}