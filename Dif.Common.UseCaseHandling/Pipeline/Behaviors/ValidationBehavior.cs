namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

using Dif.Common.UseCaseHandling.Validation;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();

        if (validator == null)
        {
            return await next();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<TResponse>(validationResult.Errors);
        }

        return await next();
    }
}
