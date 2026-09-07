using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Infrastructure.KnowledgeExtraction.Gemini;
using Baseera.Infrastructure.KnowledgeResolution.Gemini;
using Baseera.Infrastructure.KnowledgeResolution.Neo4j;
using Google.GenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Baseera.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<GeminiOptions>(options =>
            {
                options.ApiKey =
                    Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                    ?? throw new InvalidOperationException(
                        "GEMINI_API_KEY was not found.");
            });

            services.AddSingleton(sp =>
            {
                var options =
                    sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

                return new Client(apiKey: options.ApiKey);
            });

            services.AddScoped<
                IKnowledgeExtractor,
                GeminiKnowledgeExtractor>();

            services.AddSingleton<
                IEmbeddingGenerator,
                GeminiEmbeddingGenerator>();

            services.Configure<Neo4jOptions>(
                configuration.GetSection("Neo4j"));

            services.AddSingleton<IDriver>(sp =>
            {
                var options = sp
                    .GetRequiredService<IOptions<Neo4jOptions>>()
                    .Value;

                return GraphDatabase.Driver(
                    options.Uri,
                    AuthTokens.Basic(
                        options.Username,
                        options.Password));
            });

            return services;
        }
    }
}