using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiGateway.Middlewares
{
    public class ErrorWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (AuthenticationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsJsonAsync(new
                {
                    Error = ex.Message
                });
            }
        }
    }
}