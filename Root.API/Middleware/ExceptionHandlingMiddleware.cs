using System.Diagnostics;
using System.Text.Json;
using Root.API.Application.Common.Exceptions;
using Root.API.Contracts.Responses;
using AppValidationException = Root.API.Application.Common.Exceptions.ValidationException;

namespace Root.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, code, message, errors) = exception switch
        {
            AppValidationException ve => (
                StatusCodes.Status400BadRequest,
                "validation_error",
                ve.Message,
                ve.Errors.Select(e => new ValidationErrorItem { Field = e.Field, Message = e.Message }).ToList()
                    as IReadOnlyList<ValidationErrorItem>
            ),
            DomainException de => (
                StatusCodes.Status400BadRequest,
                de.Code,
                de.Message,
                (IReadOnlyList<ValidationErrorItem>?)null
            ),
            ForbiddenException fe => (
                StatusCodes.Status403Forbidden,
                "forbidden",
                fe.Message,
                (IReadOnlyList<ValidationErrorItem>?)null
            ),
            NotFoundException nfe => (
                StatusCodes.Status404NotFound,
                "not_found",
                nfe.Message,
                (IReadOnlyList<ValidationErrorItem>?)null
            ),
            ConflictException ce => (
                StatusCodes.Status409Conflict,
                ce.Code,
                ce.Message,
                (IReadOnlyList<ValidationErrorItem>?)null
            ),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "unauthorized",
                "Authentication is required.",
                (IReadOnlyList<ValidationErrorItem>?)null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "internal_server_error",
                "An unexpected error occurred.",
                (IReadOnlyList<ValidationErrorItem>?)null
            )
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception. TraceId={TraceId}", traceId);
        else
            _logger.LogWarning(exception, "Handled exception [{Code}]. TraceId={TraceId}", code, traceId);

        var response = new ErrorResponse
        {
            Code = code,
            Message = message,
            TraceId = traceId,
            Errors = errors
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
