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
using Baseera.Infrastructure.Connectors.Reddit;
using Baseera.Infrastructure.Connectors.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.Abstractions;
using Baseera.Infrastructure.Connectors.Reddit.Clients;
using Baseera.Infrastructure.Connectors.Reddit.Mappers;
using Baseera.Infrastructure.Connectors.Mastodon;
using Baseera.Infrastructure.Connectors.Mastodon.Mappers;
using Baseera.Infrastructure.Connectors.Bluesky;
using Baseera.Infrastructure.Connectors.Bluesky.Mappers;
using Baseera.Infrastructure.Connectors.Facebook;
using Baseera.Infrastructure.Connectors.Facebook.Abstractions;
using Baseera.Infrastructure.Connectors.Facebook.Clients;
using Baseera.Infrastructure.Connectors.Facebook.Mappers;

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

            services.Configure<RedditApiOptions>(
                configuration.GetSection("RedditApi"));

            services.AddHttpClient<IRedditPostClient, RedditPostClient>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<RedditApiOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.AddHttpClient<IRedditCommentClient, RedditCommentClient>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<RedditApiOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });


            services.AddSingleton(sp =>
            {
                var options =
                    sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

                return new Client(apiKey: options.ApiKey);
            });


           services.Configure<FacebookApiOptions>(
                configuration.GetSection("FacebookApi"));

            services.AddHttpClient<FacebookApiClient>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<FacebookApiOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.AddScoped<IFacebookMapper, FacebookMapper>();
            services.AddScoped<FacebookConnector>();



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

            services.AddScoped<ICandidateProvider, Neo4jCandidateProvider>();

            services.AddScoped<IResolutionJudge, GeminiResolutionJudge>();

            services.AddScoped<IRedditMapper, RedditMapper>();

            services.AddScoped<IConnector, RedditConnector>();

            services.AddScoped<IMastodonMapper, MastodonMapper>();

            services.AddHttpClient<MastodonConnector>();

            services.AddScoped<IBlueskyMapper, BlueskyMapper>();
            services.AddHttpClient<BlueskyConnector>(client =>
            {
                client.BaseAddress =
                    new Uri("https://public.api.bsky.app");
            });

            
            return services;
        }
    }
}
