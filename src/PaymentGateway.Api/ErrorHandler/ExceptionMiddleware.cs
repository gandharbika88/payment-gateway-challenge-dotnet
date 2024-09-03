using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.Api.ErrorHandler
{
    /// <summary>
    /// Represents the middleware to handle exceptions.
    /// </summary>
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        /// <summary>
        /// Invokes the middleware to handle the HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ProblemDetails
            {
                Title = "An error occurred",
                Status = context.Response.StatusCode,
                Detail = exception.Message
            };
            var result = JsonSerializer.Serialize(response);
            logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            await context.Response.WriteAsync(result);
        }
    }
}
