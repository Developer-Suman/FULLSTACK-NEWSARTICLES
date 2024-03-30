using Master_DAL.CustomExceptionMiddleware;
using Master_DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeatures != null)
                    {
                        Console.WriteLine($"Something Went Wrong:{contextFeatures.Error}");
                        await context.Response.WriteAsync(new ErrorsDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server error"

                        }.ToString());
                    }

                });
            });

        }

        public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
