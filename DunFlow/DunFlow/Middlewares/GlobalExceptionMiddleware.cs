using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DunFlow.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleGlobalExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/problem+json";

            var statusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var title = statusCode switch
            {
                StatusCodes.Status404NotFound => "Resource Not Found",
                StatusCodes.Status400BadRequest => "Bad Request",
                _ => "Internal Server Error"
            };

            var detail = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : ex.Message;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            var json = JsonConvert.SerializeObject(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
