using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Baseera.Infrastructure.KnowledgeResolution.Neo4j
{
    public sealed class Neo4jDriverFactory
    {
        private readonly Neo4jOptions _options;

        public Neo4jDriverFactory(
            IOptions<Neo4jOptions> options)
        {
            _options = options.Value;
        }

        public IDriver Create()
        {
            return GraphDatabase.Driver(
                _options.Uri,
                AuthTokens.Basic(
                    _options.Username,
                    _options.Password));
        }
    }
}
