namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

using FluentResults;

using Microsoft.Extensions.DependencyInjection;

using Dif.Common.ResultExtensions;
using Dif.Common.UseCaseHandling.Handlers;
using Dif.Common.UseCaseHandling.Messaging;

public class PublishBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IUseCase
{
    private readonly IServiceProvider _serviceProvider;

    public PublishBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        if ( result.IsSuccess && ShouldPublish(request) )
        {
            var publishResult = await TryPublishAsync(request, result.Value, cancellationToken);
            var warningsAndError = publishResult.GetAllWarningMessages();

            if ( publishResult.IsFailed )
            {
                warningsAndError.Add(publishResult.GetErrorMessage());
            }

            if ( warningsAndError.Count > 0 )
            {
                result.WithWarnings(warningsAndError);
            }
        }

        return result;
    }

    private static bool ShouldPublish(TRequest request)
    {
        if ( request is ICommand ) return true;
        if ( request is IQuery<TResponse> query ) return query.ShouldPublish;
        return false;
    }

    private async Task<Result> TryPublishAsync(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        try
        {
            Result publishResult;
            var typedPublisher = _serviceProvider.GetService<IRequestPublisher<TRequest, TResponse>>();

            if ( typedPublisher is not null )
            {
                publishResult = await typedPublisher.PublishAsync(request, response, cancellationToken);
            }
            else
            {
                var voidPublisher = _serviceProvider.GetService<IRequestPublisher<TRequest>>();
                if ( voidPublisher is null )
                    return Result.Fail($"Publisher for {request.GetType().Name} not found");

                publishResult = await voidPublisher.PublishAsync(request, cancellationToken);
            }

            return publishResult;
        }
        catch ( Exception ex )
        {
            return Result.Fail($"Faild to execute publisher for {request.GetType().Name}: {ex}");
        }
    }
}