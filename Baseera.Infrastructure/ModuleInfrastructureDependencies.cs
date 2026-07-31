using Baseera.Infrastructure.Connectors.Reddit.Clients;
using Baseera.Infrastructure.Options;
using Baseera.Service.Connectors.Reddit.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Baseera.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RedditOptions>(
                configuration.GetSection(RedditOptions.SectionName));

            services.AddRedditClient<IRedditSearchClient, RedditSearchClient>();
            services.AddRedditClient<IRedditPostClient, RedditPostClient>();
            //services.AddRedditClient<IRedditCommentClient, RedditCommentClient>();

            return services;
        }

        private static IHttpClientBuilder AddRedditClient<TInterface, TImplementation>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        {
            return services.AddHttpClient<TInterface, TImplementation>(
                (provider, client) =>
                {
                    var options = provider
                        .GetRequiredService<IOptions<RedditOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);

                    client.DefaultRequestHeaders.Add(
                        "x-rapidapi-key",
                        options.ApiKey);

                    client.DefaultRequestHeaders.Add(
                        "x-rapidapi-host",
                        options.ApiHost);
                });
        }
    }
}