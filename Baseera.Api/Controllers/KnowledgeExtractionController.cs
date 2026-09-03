using Baseera.Core.KnowledgeExtraction.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeExtractionController : ControllerBase
    {
        private readonly IKnowledgeExtractor _extractor;

        public KnowledgeExtractionController(
            IKnowledgeExtractor extractor)
        {
            _extractor = extractor;
        }

        [HttpPost]
        public async Task<IActionResult> Extract(string text)
        {
            var result =
                await _extractor.ExtractAsync(text);

            return Ok(result);
        }
    }
}
