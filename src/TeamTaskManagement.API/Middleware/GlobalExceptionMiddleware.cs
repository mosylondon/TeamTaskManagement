using System.Net;
using System.Text.Json;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.API.Middleware
{
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
                var correlationId = Guid.NewGuid().ToString();

                _logger.LogError(ex, "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}, User: {User}",
                    correlationId, context.Request.Path, context.Request.Method, context.User?.Identity?.Name ?? "Anonymous");

                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                DomainException domainEx => (HttpStatusCode.BadRequest, domainEx.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request data"),
                _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
            };

            var response = new
            {
                message,
                correlationId,
                timestamp = DateTime.UtcNow,
                details = exception is DomainException ? null : "Please contact support if the issue persists"
            };

            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
