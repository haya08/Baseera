using Baseera.Infrastructure.Connectors.Mastodon;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/mastodon")]
public sealed class MastodonConnectorController : ControllerBase
{
    private readonly MastodonConnector _connector;

    public MastodonConnectorController(
        MastodonConnector connector)
    {
        _connector = connector;
    }

    [HttpPost]
    public async Task<IActionResult> Connect(
        [FromBody] MastodonConnectRequest request,
        CancellationToken cancellationToken)
    {
        var documents = await _connector.GetDocumentsAsync(
            request.Url,
            cancellationToken);

        return Ok(documents);
    }
}

public sealed class MastodonConnectRequest
{
    public required string Url { get; init; }
}