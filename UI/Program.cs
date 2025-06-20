using Application.Delegates;
using Application.Features.Behaviors;
using Application.Features.MenuItem.Commands;
using Application.Interfaces;
using Application.Services;
using Application.Validators.MenuItem;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using BLL.Services;
using DAL.Data;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using DAL.Parameters;
using DAL.Repositories;
using DAL.SharedKernels;
using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Serilog;
using UI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddTransient<PostgresCacheService>();
builder.Services.AddTransient<RedisCacheService>();

builder.Services.AddScoped<CacheServiceResolver>(serviceProvider => key =>
{
    switch (key)
    {
        case CachingType.Postgres:
            return serviceProvider.GetRequiredService<PostgresCacheService>();
        case CachingType.Redis:
            return serviceProvider.GetRequiredService<RedisCacheService>();
        default:
            throw new KeyNotFoundException();
    }
});

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis"))
    .AddNpgSql(builder.Configuration.GetConnectionString("Database"))
    .AddDbContextCheck<AppReadDbContext>()
    .AddDbContextCheck<AppWriteDbContext>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IMenuItemRepository>()
    .AddClasses(classes => classes
        .AssignableTo<IWebhookEventRepository>()
        .AssignableTo<IMenuItemRepository>()
        .AssignableTo<IUnitOfWork>()
    )
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IMenuItemService>()
    .AddClasses(classes => classes
        .AssignableTo<IWebhookEventService>()
        .AssignableTo<IMenuItemService>()
    )
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddHttpClient<IWebhookEventDispatcher, WebhookEventDispatcher>();

builder.Services.AddDbContext<AppWriteDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddDbContext<AppReadDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(configuration =>
{
    var assemblies = typeof(CreateMenuItemCommandHandler).Assembly;
    configuration.RegisterServicesFromAssemblies(assemblies);
    configuration.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "online-sushi-bar-api:";
});

builder.Services.Configure<CloudStorageParameters>(builder.Configuration.GetSection("CloudStorageSettings"));

builder.Services.AddRateLimiterExtention();

builder.Services.AddResiliencePipeline<string, Result<GetMenuItemDto>>("menu-items-fallback",
    pipelineBuilder =>
    {
        pipelineBuilder.AddFallback(new FallbackStrategyOptions<Result<GetMenuItemDto>>
        {
            FallbackAction = _ => Outcome.FromResultAsValueTask<Result<GetMenuItemDto>>(Result<GetMenuItemDto>.Success(new GetMenuItemDto())),
        });
    });
builder.Services.AddResiliencePipeline<string, WebhookEvent>("webhook-events-fallback",
    pipelineBuilder =>
    {
        pipelineBuilder.AddRetry(new RetryStrategyOptions<WebhookEvent>
        {
            MaxRetryAttempts = 3
        });
    });

builder.Services.AddValidatorsFromAssembly(typeof(CreateMenuItemDtoValidator).Assembly, includeInternalTypes: true);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://localhost:7274")
                .SetIsOriginAllowed(origin => true);
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Online Sushi Bar API", Version = "v1" });
});

var app = builder.Build();

await ApplyDatabaseMigration(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCors();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.MapControllers();

app.Run();

static async Task ApplyDatabaseMigration(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppWriteDbContext>();

    await db.Database.ExecuteSqlRawAsync(
        """
        CREATE UNLOGGED TABLE IF NOT EXISTS cache (
            id SERIAL PRIMARY KEY,
            key TEXT UNIQUE NOT NULL,
            value JSONB,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
               
        CREATE INDEX IF NOT EXISTS idx_cache_key ON cache (key);
        """);
}