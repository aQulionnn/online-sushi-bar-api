using MediatR;
using Serilog;

namespace Application.Features.Behaviors
{
    public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;

            Log.Information($"Processing request {requestName}");

            TResponse response = await next();

            Log.Information($"Request {requestName} succeeded");

            return response;
        }
    }
}
