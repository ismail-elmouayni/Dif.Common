# Dif.Common.UseCaseHandling

`Dif.Common.UseCaseHandling` provides a scoped mediator for dispatching commands and queries through a configurable pipeline. Handlers, validators, and request publishers return [FluentResults](https://github.com/altmann/FluentResults) results.

## Add the project reference

From the repository root, add the library to your application:

```sh
dotnet add path/to/YourApp.csproj reference ./Dif.Common.UseCaseHandling/Dif.Common.UseCaseHandling.csproj
```

The project targets .NET 8. Its package versions are managed by the repository's `Directory.Packages.props`.

## Register the mediator

Register the assembly containing your handlers. The default pipeline includes logging, exception handling, validation, publishing, and atomic transaction behaviors. Handler, validator, and publisher implementations in the supplied assemblies are registered as scoped services.

```csharp
using Dif.Common.UseCaseHandling.Extensions;

builder.Services.AddMediatorPipeline(typeof(CreateOrderHandler).Assembly);
```

Resolve `IMediator` from a dependency-injection scope, such as an ASP.NET Core request scope. If an application does not use the atomic behavior, use `AddNonTransactionalMediatorPipe` instead:

```csharp
builder.Services.AddNonTransactionalMediatorPipe(typeof(CreateOrderHandler).Assembly);
```

## Define a command or query

Use `ICommand` for a write operation, and `IQuery<TResponse>` for a read operation. `ICommand` requires a `CommandType` property.

```csharp
using Dif.Common.UseCaseHandling.Handlers;

public sealed record CreateOrderCommand(string CustomerId) : ICommand
{
    public string CommandType => nameof(CreateOrderCommand);
}

public sealed record GetOrderQuery(Guid Id) : IQuery<OrderDto>;

public sealed record OrderDto(Guid Id, string CustomerId);
```

Implement the matching handler interface. Commands can return either a value or a plain `Result`; queries return `Result<TResponse>`.

```csharp
using Dif.Common.UseCaseHandling.Handlers;
using FluentResults;

public sealed class CreateOrderHandler
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        return Task.FromResult(Result.Ok(id));
    }
}

public sealed class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    public Task<Result<OrderDto>> HandleAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken)
    {
        // Load the order from the application's data store.
        var order = new OrderDto(query.Id, "customer-id");
        return Task.FromResult(Result.Ok(order));
    }
}
```

## Dispatch a request

Use the generic arguments to specify the response type for a query or value-returning command. A command without a response uses the single-type overload.

```csharp
using Dif.Common.UseCaseHandling.Pipeline;

public sealed class OrderService(IMediator mediator)
{
    public async Task<Guid> CreateAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync<CreateOrderCommand, Guid>(
            new CreateOrderCommand(customerId),
            cancellationToken);

        if (result.IsFailed)
        {
            // Inspect result.Errors or return the failure to the caller.
            throw new InvalidOperationException(result.Errors[0].Message);
        }

        return result.Value;
    }

    public Task<Result<OrderDto>> GetAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        mediator.SendAsync<GetOrderQuery, OrderDto>(
            new GetOrderQuery(id),
            cancellationToken);
}
```

The mediator returns `Result` or `Result<T>` rather than translating failures into exceptions. A `DomainException` thrown during handling is converted to a failed result; other exceptions are logged and rethrown.

## Validate requests

Implement `IValidator<TRequest>` for a request. The validation behavior resolves one validator for that request and skips the handler when validation fails. Combine a request's rules in one validator.

```csharp
using Dif.Common.ResultExtensions.Errors;
using Dif.Common.UseCaseHandling.Validation;
using FluentResults;

public sealed class CreateOrderValidator : IValidator<CreateOrderCommand>
{
    public Task<Result> ValidateAsync(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            return Task.FromResult<Result>(
                Result.Fail(new ValidationError(
                    "CustomerId is required.",
                    nameof(request.CustomerId))));
        }

        return Task.FromResult(Result.Ok());
    }
}
```

Validators are discovered when their assembly is passed to `AddMediatorPipeline` or `AddNonTransactionalMediatorPipe`.

## Publishing and transactions

- Successful requests implementing `ICommand` are sent to a registered `IRequestPublisher<TRequest, TResponse>` or `IRequestPublisher<TRequest>`. If no publisher is registered, the command result stays successful but receives a warning. Queries publish only when `ShouldPublish` is overridden to return `true`.
- Requests implementing `ITransactionalRequest` use `AtomicBehavior`. Register an `IUnitOfWork` implementation for these requests. The behavior saves and commits on success and rolls back on a failed result or exception. Requests without the marker do not open a transaction.
- `AddNonTransactionalMediatorPipe` omits `AtomicBehavior`; use it when the application does not provide unit-of-work transactions.

## Proposed repository and application structure

The following is a possible future layout for this library repository. The current projects can remain unchanged until there is a reason to move them under `src/`.

```text
Dif.Common.sln
Directory.Build.props
Directory.Packages.props
src/
    Dif.Common.ResultExtensions/
    Dif.Common.UseCaseHandling/
tests/
    Dif.Common.ResultExtensions.Tests/
    Dif.Common.UseCaseHandling.Tests/
samples/
    Dif.Common.UseCaseHandling.Sample/
```

In an application using the library, organize code by use case and keep each command's request, handler, validator, and optional publisher together. This follows the referenced Rolling Pass Tracker application's `UseCases/Commands` and `UseCases/Queries` organization:

```text
YourProduct.Application/
    UseCases/
        Commands/
            AddProduct/
                AddProductCommand.cs
                AddProductCommandHandler.cs
                AddProductCommandValidator.cs
                AddProductCommandPublisher.cs
        Queries/
            GetProducts/
                GetProductsQuery.cs
                GetProductsQueryHandler.cs
                GetProductsQueryValidator.cs
YourProduct.Infrastructure/
    Messaging/
        MessagePublusher.cs
```

Keep the publisher beside the command it handles rather than collecting unrelated publishers in a global folder. 

The current `PublishBehavior` tries to publish every successful `ICommand`; queries publish only when `IQuery<TResponse>.ShouldPublish` is true. Therefore, under the current behavior, each command should have a matching publisher or its result receives a warning. A future improvement would make command publishing explicitly opt-in, so ordinary write commands do not attempt publication just because they implement `ICommand`.

For transactional commands, the current pipeline commits the unit of work before it invokes the publisher. A publisher failure is added as a warning and does not undo the committed command. That is suitable only when best-effort delivery is acceptable. For reliable integration events, prefer a transactional outbox: store the event in the same transaction as the state change, then dispatch it from a background worker with retry and idempotency handling.

## ASP.NET Core endpoints

The optional endpoint helpers bind requests, dispatch through `IMediator`, and convert results to HTTP responses.

```csharp
using Dif.Common.UseCaseHandling.AspNet;

app.MapPostCommand<CreateOrderCommand, Guid>("/orders");
app.MapGetQuery<GetOrderQuery, OrderDto>("/orders/{id}");
```

Successful value results map to `200 OK`, and successful void results to `204 No Content`. Failures map based on the first error: `ValidationError` to `400`, `NotFoundError` to `404`, `ConflictError` to `409`, unauthorized metadata to `401`, and other errors to `500`.