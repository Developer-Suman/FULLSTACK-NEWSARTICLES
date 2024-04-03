using Master_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

            }
            catch (Exception ex)
            {
                 _logger.LogInformation(ex.Message);
                 await HandleExceptionAsync(httpContext, ex);

            }

        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var message = ex switch
            {
                AccessViolationException => new { Message = "Access Error", StatusCode = 5001 },
                DivideByZeroException => new { Message = "Divide by Zero from Custom Middleware", StatusCode = 5002 },
                NotImplementedException => new { Message = "501 - Not Implemented", StatusCode = 501 },
                HttpRequestException httpEx when httpEx.Message.Contains("Bad Gateway") => new {Message="502- BadGateWay from External Server", StatusCode=502},
                //ServiceUnavailableException => new { Message = "503 - Service Unavailable", StatusCode = 503 },
                _ => new { Message = "Internal Server error from the custom middleware", StatusCode = 500 }


            };

            await httpContext.Response.WriteAsync(new ErrorsDetails
            {
                StatusCode = message.StatusCode,
                Message = message.Message

            }.ToString());
        }
    }
}
