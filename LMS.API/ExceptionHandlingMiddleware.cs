using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LMS.API
{
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
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle exception and log it
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Handle the error and return a response
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Status = statusCode 
            };

            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = "Here you can include specific details about what went wrong.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = "You do not have permission to access this resource.";
            }
            else if (exception is DbUpdateConcurrencyException)
            {
                statusCode = StatusCodes.Status409Conflict;
                problemDetails.Title = "Concurrency Conflict";
                problemDetails.Detail = "A concurrency issue occurred while processing your request.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = "The requested resource could not be found.";
            }
            else if (exception is TimeoutException)
            {
                statusCode = StatusCodes.Status408RequestTimeout;
                problemDetails.Title = "Request Timeout";
                problemDetails.Detail = "The server timed out waiting for the request.";
            }
            else if (exception is NotImplementedException)
            {
                statusCode = StatusCodes.Status501NotImplemented;
                problemDetails.Title = "Not Implemented";
                problemDetails.Detail = "This functionality is not yet implemented.";
            }
            else
            {
                // Default values if exeption not catched
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
            }

            problemDetails.Status = statusCode;
            context.Response.StatusCode = statusCode;

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
