using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.CustomExceptionMiddleware
{
    public static class HttpContextExtensions
    {
        public static string GetControllerName(this HttpContext context)
        {
            var controllerActionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if(controllerActionDescriptor is not null)
            {
                return controllerActionDescriptor.ControllerName;
            }
            return "UnknownControllerName";

        }
    }
}
