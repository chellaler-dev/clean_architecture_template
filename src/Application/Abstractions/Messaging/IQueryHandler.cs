using MediatR;
using SharedKernel;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Adding this level of abstraction:
    /// Makes intent explicit (Command vs Query).
    /// Enforces consistent return type (Result / Result<T>).
    /// Keeps application layer decoupled from libraries like MediatR.
    /// Improves testability and maintainability.
    /// Supports generic pipelines and behaviors consistently across the application.
/// </summary>

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
