using System.Net;
using SmartHomeProject.Common;

namespace SmartHomeProject.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found");
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "Resource not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation");
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid operation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Error(
                new List<string> { message },
                (int)statusCode);

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}