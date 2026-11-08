namespace Dif.Common.UseCaseHandling.Pipeline.Behaviors;

[Obsolete("Use AtomicBehavior instead.")]
public class TransactionBehavior<TRequest, TResponse> : AtomicBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public TransactionBehavior(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        : base(serviceProvider, logger) { }
}
