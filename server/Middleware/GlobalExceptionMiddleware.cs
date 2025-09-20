using System.Net;
using System.Text.Json;

namespace TodoApp.Server.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Invalid argument provided";
                response.Details = argEx.Message;
                break;
            case ArgumentNullException nullEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Required parameter is missing";
                response.Details = nullEx.Message;
                break;
            case KeyNotFoundException keyEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Resource not found";
                response.Details = keyEx.Message;
                break;
            case InvalidOperationException opEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = "Operation cannot be completed";
                response.Details = opEx.Message;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred";
                response.Details = "Please try again later";
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
