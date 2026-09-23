namespace Baseera.Infrastructure.Options
{
    public class Neo4jOptions
    {
        public string Uri { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Database { get; set; } = "neo4j";
    }
}
