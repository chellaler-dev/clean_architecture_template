using System.Diagnostics.CodeAnalysis;

namespace SharedKernel;

/// <summary>
/// Functional-style error handling pattern
/// Returned by a handler in the application layer (success or failure).
/// Interpreted by the endpoint in the presentation layer to produce the appropriate response for the client.
/// Forces consumersto handle success vs failure instead of relying on exceptions.
/// Works with value-returning operations (Result<TValue>) or operations with no return value
/// </summary>
public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        if (!isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    // Implicit conversion: a EntityResponse object can automatically be converted to Result<EntityResponse>.
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
