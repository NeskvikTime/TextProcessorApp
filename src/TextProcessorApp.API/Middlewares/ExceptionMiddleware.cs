using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using TextProcessorApp.Contracts.Exceptions;

namespace TextProcessorApp.API.Middlewares;

public class ExceptionMiddleware : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ExceptionMiddleware(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemException problemException)
        {
            return true;
        }

        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = exception.Message,
            Type = "Bad Request"
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext() 
            { 
                HttpContext = httpContext, 
                ProblemDetails = problemDetails
            });
    }
}
