using System.Net;

namespace HospitalRegistry.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await this.HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;
            switch (exception)
            {
                case KeyNotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case ArgumentNullException:
                case ArgumentException argumentException:
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            httpContext.Response.ContentType = "application/text";
            httpContext.Response.StatusCode = (int)code;
            if (result == string.Empty)
            {
                result = exception.Message;
            }

            return httpContext.Response.WriteAsync(result);
        }
    }

    public static class AppBuilderExt
    {
        public static IApplicationBuilder UseExceptionsHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
