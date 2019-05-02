using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Questions.Managers
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public GlobalExceptionHandling(RequestDelegate next,ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory?.CreateLogger<Exception>();

        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, new object[] { });
                await httpContext.Response.WriteAsync("Invalid Operation");                 
            } 

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandling>();
        }
    }
}
