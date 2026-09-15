using Baseera.Infrastructure.Connectors.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/facebook")]
public sealed class FacebookConnectorController : ControllerBase
{
    private readonly FacebookConnector _connector;

    public FacebookConnectorController(
        FacebookConnector connector)
    {
        _connector = connector;
    }

    [HttpPost]
    public async Task<IActionResult> Connect(
        [FromBody] FacebookConnectRequest request,
        CancellationToken cancellationToken)
    {
        var documents =
            await _connector.GetDocumentsAsync(
                request.Url,
                cancellationToken);

        return Ok(documents);
    }
}

public sealed class FacebookConnectRequest
{
    public required string Url { get; init; }
}