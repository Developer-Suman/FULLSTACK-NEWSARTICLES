using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Exceptions
{
    public class ImageUrlException : Exception
    {
        public ImageUrlException()
        {
            
        }

        public ImageUrlException(string message) : base(message)
        {
            
        }

        public ImageUrlException(string message, Exception inner): base(message, inner)
        {
            
        }

        public ImageUrlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context): base(info, context)
        {
            
        }
    }
}
