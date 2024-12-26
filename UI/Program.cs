using Application.Features.Behaviors;
using Application.Features.MenuItem.Commands;
using Application.Interfaces;
using Application.Services;
using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.Parameters;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Fallback;
using Serilog;
using UI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IRedisService, RedisService>();

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

builder.Services.AddResiliencePipeline<string, GetMenuItemDto>("menu-items-fallback",
    pipelineBuilder =>
    {
        pipelineBuilder.AddFallback(new FallbackStrategyOptions<GetMenuItemDto>
        {
            FallbackAction = _ => Outcome.FromResultAsValueTask<GetMenuItemDto>(new GetMenuItemDto()),
        });
    });

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.MapControllers();

app.Run();
