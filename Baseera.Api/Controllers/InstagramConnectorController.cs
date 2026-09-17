using Baseera.Infrastructure.Connectors.Instagram;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/instagram")]
public sealed class InstagramConnectorController : ControllerBase
{
    private readonly InstagramConnector _connector;

    public InstagramConnectorController(
        InstagramConnector connector)
    {
        _connector = connector;
    }

    [HttpPost]
    public async Task<IActionResult> Connect(
        [FromBody] InstagramConnectRequest request,
        CancellationToken cancellationToken)
    {
        var documents =
            await _connector.GetDocumentsAsync(
                request.Url,
                cancellationToken);

        return Ok(documents);
    }
}

public sealed class InstagramConnectRequest
{
    public string Url { get; set; } = string.Empty;
}