using System.Text.Json.Serialization;

namespace Baseera.Infrastructure.Connectors.Facebook.ApiModels;

public sealed class FacebookPost
{
    public string Id { get; init; } = string.Empty;

    public string? Message { get; init; }

    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; init; }

    [JsonPropertyName("permalink_url")]
    public string? PermalinkUrl { get; init; }

    [JsonPropertyName("full_picture")]
    public string? FullPicture { get; init; }

    public FacebookAttachments? Attachments { get; init; }

    public FacebookShares? Shares { get; init; }
}

public sealed class FacebookAttachments
{
    public IReadOnlyList<FacebookAttachment> Data { get; init; } = [];
}

public sealed class FacebookAttachment
{
    public string? Description { get; init; }

    public FacebookMedia? Media { get; init; }

    public FacebookTarget? Target { get; init; }

    public string? Type { get; init; }

    public string? Url { get; init; }
}

public sealed class FacebookMedia
{
    public FacebookImage? Image { get; init; }

    public FacebookVideo? Video { get; init; }
}

public sealed class FacebookImage
{
    public int? Height { get; init; }

    public string? Src { get; init; }

    public int? Width { get; init; }
}

public sealed class FacebookVideo
{
    public string? Source { get; init; }

    public int? Height { get; init; }

    public int? Width { get; init; }
}

public sealed class FacebookTarget
{
    public string? Id { get; init; }

    public string? Url { get; init; }
}

public sealed class FacebookShares
{
    public int Count { get; init; }
}