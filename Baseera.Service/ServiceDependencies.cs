using Baseera.Core.KnowledgePipeline.Abstracts;
using Baseera.Core.KnowledgeResolution.Abstracts;
using Baseera.Service.KnowledgePipeline;
using Baseera.Service.KnowledgeResolution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baseera.Service
{
    public static class ServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IKnowledgeResolver, KnowledgeResolutionService>();

            services.AddScoped<IKnowledgePipeline, KnowledgePipelineService>();

            return services;
        }
    }
}
