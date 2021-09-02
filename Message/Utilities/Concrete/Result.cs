using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Message.Utilities.Abstract;
namespace Message.Utilities.Concrete
{
    public class Result<T> : IResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Result(bool success, string message, T data)
        {
            Data = data;
            Success = success;
            Message = message;
        }
        public Result(bool success, T data)
        {
            Success = success;
            Data = data;
        }
        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public Result(bool success)
        {
            Success = success;
        }
    }
}