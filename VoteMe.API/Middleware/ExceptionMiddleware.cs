using System.Net;
using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Domain.Exceptions;

namespace VoteMe.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex.Message);
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Unauthorized);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex.Message);
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex.Message);
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, "Something went wrong on our end", HttpStatusCode.InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.FailureResponse(message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
