using Baseera.Infrastructure;
using System.Text.Json.Serialization;
using Baseera.Infrastructure.Auth.Meta;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// Load .env from solution root
DotNetEnv.Env.Load(
    Path.Combine(builder.Environment.ContentRootPath, "..", ".env")
);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.Configure<MetaOAuthOptions>(
    builder.Configuration.GetSection("MetaOAuth"));

builder.Services.AddHttpClient<IMetaOAuthService, MetaOAuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Dependency Injection

builder.Services.AddInfrastructureDependencies(
    builder.Configuration);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "v1");

        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();