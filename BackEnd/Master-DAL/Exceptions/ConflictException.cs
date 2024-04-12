using Azure.Core;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException()
        {
            
        }
        
        public ConflictException(string message) : base(message)
        {
            
        }

        public HttpStatusCode StatusCode { get { return HttpStatusCode.Conflict; } }
    }
}
