using Baseera.Infrastructure.Connectors.Bluesky;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/bluesky")]
public sealed class BlueskyConnectorController : ControllerBase
{
    private readonly BlueskyConnector _connector;

    public BlueskyConnectorController(
        BlueskyConnector connector)
    {
        _connector = connector;
    }

    [HttpPost]
    public async Task<IActionResult> Connect(
        [FromBody] BlueskyConnectRequest request,
        CancellationToken cancellationToken)
    {
        var documents =
            await _connector.GetDocumentsAsync(
                request.Url,
                cancellationToken);

        return Ok(documents);
    }
}

public sealed class BlueskyConnectRequest
{
    public required string Url { get; init; }
}