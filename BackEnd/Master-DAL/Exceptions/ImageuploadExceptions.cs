using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Exceptions
{
    public class ImageuploadExceptions : Exception
    {
        //Default Constructor
        public ImageuploadExceptions()
        {
            
        }

        //Parametarized Constructor
        public ImageuploadExceptions(string message) : base(message)
        {

        }

        //Constructor with messsage and inner exception
        public ImageuploadExceptions(string message, Exception inner) : base(message, inner)
        {
            
        }

        //constructor with serilizationinfo and streamingcontext
        public ImageuploadExceptions(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context): base(info, context)
        {
            
        }

    }
}
