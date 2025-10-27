using FluentValidation;
using MediatR;

namespace Application.Behaviours;

public record ValidationError(string PropertyName, string ErrorMessage);
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : MediatR.IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {


        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context)));


        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();
        if (errors.Any())
        {
            var errorDictionary = errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.ErrorMessage).ToArray());

            var validationErrorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
            
            throw new ValidationException(validationErrorMessage);
        }
        var response = await next();

        return response;
    }
}

