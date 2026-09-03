using Baseera.Core.KnowledgeExtraction.Abstracts;
using Baseera.Infrastructure.KnowledgeExtraction.Gemini;
using Google.GenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                configuration
                    .GetSection("Gemini")
                    .Bind(options);
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

            return services;
        }
    }
}