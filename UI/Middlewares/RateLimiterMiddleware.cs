using System.Threading.RateLimiting;

namespace UI.Middlewares
{
    public static class RateLimiterMiddleware
    {
        public static IServiceCollection AddRateLimiterExtention(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("PostRequestLimiter", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(5),
                        QueueLimit = 5
                    })
                );

                options.AddPolicy("GetRequestLimiter", httpsContext =>
                    RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpsContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100, 
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 10, 
                        QueueLimit = 0 
                    })
                );

                options.AddPolicy("PutRequestLimiter", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 1,
                    })
                );

                options.AddPolicy("DeleteRequestLimiter", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(1),
                        QueueLimit = 1
                    })
                );
            });

            return services;
        }
    }
}
