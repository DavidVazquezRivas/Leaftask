using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "[{Behavior}] Executing request: {RequestName} with data {@Request}",
            nameof(LoggingBehavior<TRequest, TResponse>),
            requestName,
            request);

        Stopwatch timer = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        timer.Stop();

        logger.LogInformation(
            "[{Behavior}] Request {RequestName} completed successfully in {ElapsedMilliseconds}ms",
            nameof(LoggingBehavior<TRequest, TResponse>),
            requestName,
            timer.ElapsedMilliseconds);

        return response;
    }
}
