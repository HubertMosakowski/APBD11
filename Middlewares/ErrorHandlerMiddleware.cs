using System.Net;

namespace APBD11Login.Middlewares;

public class ErrorHandlerMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleWare> _logger;

    public ErrorHandlerMiddleWare(RequestDelegate next, ILogger<ErrorHandlerMiddleWare> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request.",
                detail = e.Message
            }
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
        return httpContext.Response.WriteAsync(jsonResponse);
    }
}