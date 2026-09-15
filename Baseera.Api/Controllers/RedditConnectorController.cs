using Baseera.Infrastructure.Connectors.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/reddit")]
public sealed class RedditConnectorController : ControllerBase
{
    private readonly IConnector _connector;

    public RedditConnectorController(IConnector connector)
    {
        _connector = connector;
    }

    [HttpPost]
    public async Task<IActionResult> Connect(
        [FromBody] RedditConnectRequest request,
        CancellationToken cancellationToken)
    {
        var documents = await _connector.GetDocumentsAsync(
            request.Url,
            cancellationToken);

        return Ok(documents);
    }
}

public sealed class RedditConnectRequest
{
    public required string Url { get; init; }
}