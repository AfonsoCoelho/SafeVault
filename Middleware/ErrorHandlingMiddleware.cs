using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace SafeVault.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var response = new { error = "Internal server error." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
