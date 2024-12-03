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

                options.AddPolicy("GetRequestLimiter", httpsContext =>
                    RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpsContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100, 
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 10, 
                        QueueLimit = 0 
                    }));
            });

            return services;
        }
    }
}
