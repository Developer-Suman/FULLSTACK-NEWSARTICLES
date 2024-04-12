using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            
        }

        public NotFoundException(string message) : base(message)
        {
            
        }

        //This property is used for Providing status code to client
        public HttpStatusCode StatusCode { get { return HttpStatusCode.NotFound; } }
    }
}
