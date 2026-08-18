using Microsoft.AspNetCore.Diagnostics;
using OfficeQr.Dtos.Common;
using OfficeQr.Exceptions;

namespace OfficeQr.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = "An unexpected error occurred.";

        if (exception is AppException appException)
        {
            statusCode = appException.StatusCode;
            message = appException.Message;
        }
        else if (exception is FluentValidation.ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = validationException.Message;
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception occurred");
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ApiErrorResponse { Success = false, Message = message },
            cancellationToken);

        return true;
    }
}
