using Baseera.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load .env from solution root
DotNetEnv.Env.Load(
    Path.Combine(builder.Environment.ContentRootPath, "..", ".env")
);

// Add services to the container.

builder.Services.AddControllers();

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

app.UseAuthorization();

app.MapControllers();

app.Run();