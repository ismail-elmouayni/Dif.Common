namespace Dif.Common.UseCaseHandling.Handlers;

using FluentResults;

/// <summary>
/// Handler interface for commands that produce no value.
/// Commands represent write operations in CQRS pattern.
/// </summary>
public interface ICommandHandler<TCommand>
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Handler interface for commands that produce a response value.
/// Commands represent write operations in CQRS pattern.
/// </summary>
public interface ICommandHandler<TCommand, TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
