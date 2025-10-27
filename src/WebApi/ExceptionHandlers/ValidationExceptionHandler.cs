using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace WebApi.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{

    private readonly IProblemDetailsService _problemDetailsService;


    public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = validationException.Message,
            Type = "Bad Request"
        }, Exception = exception
        });

    }
}