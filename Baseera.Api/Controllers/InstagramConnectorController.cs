using Baseera.Core.KnowledgeIngestion;
using Baseera.Infrastructure.Connectors.Instagram;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers;

[ApiController]
[Route("api/connectors/instagram")]
public sealed class InstagramConnectorController : ControllerBase
{
    private readonly InstagramConnector _connector;
    private readonly IKnowledgeIngestionService _ingestionService;

    public InstagramConnectorController(
        InstagramConnector connector,
        IKnowledgeIngestionService ingestionService)
    {
        _connector = connector;
        _ingestionService = ingestionService;
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

        var results =
            await _ingestionService.ProcessAsync(
                documents,
                cancellationToken);

        return Ok(results);
    }
}

public sealed class InstagramConnectRequest
{
    public string Url { get; set; } = string.Empty;
}