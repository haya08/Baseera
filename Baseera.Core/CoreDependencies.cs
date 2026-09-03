using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Baseera.Core
{
    public static class CoreDependencies
    {
        public static IServiceCollection AddCoreDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            return services;
        }
    }
}
