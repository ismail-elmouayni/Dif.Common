namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

using System.Diagnostics;

using FluentResults;

using Microsoft.Extensions.Logging;

using Dif.Common.ResultExtensions;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} : {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await next();
            stopwatch.Stop();

            if ( result.IsSuccess )
            {
                if ( result.GetAllWarningMessages() is { Count: > 0 } messages )
                {
                    _logger.LogWarning(
                        "Handled {RequestName} in {ElapsedMs}ms with warnings: {Warnings}",
                        requestName,
                        stopwatch.ElapsedMilliseconds,
                        messages);
                }
                else
                {
                    _logger.LogInformation(
                        "Handled {RequestName} successfully in {ElapsedMs}ms",
                        requestName,
                        stopwatch.ElapsedMilliseconds);
                }
            }
            else
            {
                _logger.LogError(
                    "Handled {RequestName} with errors in {ElapsedMs}ms - Errors: {@Errors}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    result.Errors);
            }

            return result;
        }
        catch ( Exception ex )
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMs}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}