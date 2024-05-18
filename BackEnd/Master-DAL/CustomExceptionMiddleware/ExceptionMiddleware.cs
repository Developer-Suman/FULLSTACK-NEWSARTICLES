using Master_DAL.Exceptions;
using Master_DAL.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Master_DAL.CustomExceptionMiddleware
{
    public class ExceptionMiddleware : IExceptionHandler
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

            }catch(NotFoundException ex)
            {
                await HandleExceptionAsync(httpContext, ex, ex.StatusCode);
            }
            catch(ConflictException ex)
            {
                await HandleExceptionAsync(httpContext, ex, ex.StatusCode);
            }
            catch(MappingException ex)
            {
                await HandleExceptionAsync(httpContext, ex, ex.StatusCode);
            }
            catch (Exception ex)
            {
                var controllerName = httpContext.GetControllerName();

                //var exceptionFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

                //Get Required Exception with Stack
                //_logger.LogError(ex, "This is Required Exception with Stack");
                _logger.LogError(ex, "");
                //Log the exception with Detailed Information
                _logger.LogError("An exception occured in the Controller:{controller} and Error:{ErrorMessage}",controllerName, ex.Message);
                
                //optionally log additional context;
                 _logger.LogError("Request Path:{RequestPath}, User:{UserName}", httpContext.Request.Path, httpContext.User?.Identity?.Name);
                 await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError);

                

            }
        }
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex , HttpStatusCode httpStatusCode)
        {
            httpContext.Response.StatusCode = (int)httpStatusCode;
            httpContext.Response.ContentType = "application/json";

            _logger.LogError("An exception occured:{Exception}", ex);

            await httpContext.Response.WriteAsync(new ErrorsDetails
            {
                StatusCode = (int)httpStatusCode,
                Message = ex.Message

            }.ToString());
        }
    }
}
