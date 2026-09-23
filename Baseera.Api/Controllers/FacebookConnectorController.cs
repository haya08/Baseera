using Baseera.Core.KnowledgeIngestion;
using Baseera.Infrastructure.Connectors.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/facebook")]
public sealed class FacebookConnectorController : ControllerBase
{
    private readonly FacebookConnector _connector;
    private readonly IKnowledgeIngestionService _ingestionService;

    public FacebookConnectorController(
        FacebookConnector connector,
        IKnowledgeIngestionService ingestionService)
    {
        _connector = connector;
        _ingestionService = ingestionService;
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

        var results =
            await _ingestionService.ProcessAsync(
                documents,
                cancellationToken);

        return Ok(results);
    }
}

public sealed class FacebookConnectRequest
{
    public required string Url { get; init; }
}