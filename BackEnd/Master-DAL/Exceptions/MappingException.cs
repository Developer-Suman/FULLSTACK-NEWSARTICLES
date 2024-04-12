using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Exceptions
{
    public class MappingException : Exception
    {
        public MappingException() { }

        public MappingException(string message) : base(message) { }
      

        public MappingException(string message, Exception inner) : base(message, inner)
        {
            
        }

        public MappingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context): base(info, context)
        {
            
        }

        public HttpStatusCode StatusCode { get { return HttpStatusCode.BadRequest; } }
    }
}
