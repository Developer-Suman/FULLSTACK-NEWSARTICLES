using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Master_DAL.Abstraction
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public IEnumerable<string> Errors { get; }

        public T Data { get; }

        protected Result(bool isSuccess,bool isException, IEnumerable<string> errors, T data = default!)
        {
         
            IsSuccess = isSuccess;
            Errors = errors ?? Enumerable.Empty<string>();
            Data = data;

        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, false, Enumerable.Empty<string>(), data);
        }

        public static Result<T> Success()
        {
            return new Result<T>(true,false, Enumerable.Empty<string>(), default!);
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false,false, errors);
        }

        public static Result<T> Failure(string errors)
        {
            return new Result<T>(false,false, new List<string> { errors });
        }

 
        
    }
}
