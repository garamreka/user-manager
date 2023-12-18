using System.Net;
using UserManager.Api.CustomExceptions;
using Newtonsoft.Json;

namespace UserManager.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            Error error;

            switch (exception)
            {
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    error = new Error
                    {
                        Message = "The requested resource was not found."
                    };
                    break;
                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    error = new Error
                    {
                        Message = "The request is invalid."
                    };
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error = new Error
                    {
                        Message = "An error occurred while processing your request. Please try again later."
                    };
                    break;
            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}
