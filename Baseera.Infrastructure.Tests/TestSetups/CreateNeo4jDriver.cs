using Neo4j.Driver;
using Xunit;

namespace Baseera.Infrastructure.Tests.TestSetups
{
    public static class CreateNeo4jDriver
    {
        public static async Task<IDriver> CreateDriverAsync()
        {
            TestEnvironment.Load();
            var neo4jUri =
                Environment.GetEnvironmentVariable("NEO4J_URI");
            var neo4jUsername =
                Environment.GetEnvironmentVariable("NEO4J_USERNAME");
            var neo4jPassword =
                Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
            Assert.False(
                string.IsNullOrWhiteSpace(neo4jUri),
                "NEO4J_URI is missing.");
            Assert.False(
                string.IsNullOrWhiteSpace(neo4jUsername),
                "NEO4J_USERNAME is missing.");
            Assert.False(
                string.IsNullOrWhiteSpace(neo4jPassword),
                "NEO4J_PASSWORD is missing.");
            var driver = GraphDatabase.Driver(
                neo4jUri,
                AuthTokens.Basic(
                    neo4jUsername,
                    neo4jPassword));
            // Test the connection
            await using var session = driver.AsyncSession();
            await session.RunAsync("RETURN 1");
            return driver;
        }
    }
}
