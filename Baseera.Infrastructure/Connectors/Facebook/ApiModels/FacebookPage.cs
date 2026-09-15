using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Facebook.ApiModels;

public sealed class FacebookPage
{
    public string Id { get; init; } = string.Empty;

    public string? Name { get; init; }

    public string? Link { get; init; }
}