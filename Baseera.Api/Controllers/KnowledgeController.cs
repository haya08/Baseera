using Baseera.Core.KnowledgeExtraction.Models;
using Baseera.Core.KnowledgePipeline.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeController : ControllerBase
    {
        private readonly IKnowledgePipeline _pipeline;

        public KnowledgeController(
            IKnowledgePipeline pipeline)
        {
            _pipeline = pipeline;
        }

        [HttpPost]
        public async Task<IActionResult> Process(
            UnifiedRawDocument document,
            CancellationToken cancellationToken)
        {
            var result =
                await _pipeline.ProcessAsync(
                    document,
                    cancellationToken);

            return Ok(result);
        }
    }
}
