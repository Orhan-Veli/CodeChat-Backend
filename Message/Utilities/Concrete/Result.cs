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
        public HttpStatusCode Response { get; set; }
        public Result(bool success, string message, T data,HttpStatusCode response)
        {
            Data = data;
            Success = success;
            Message = message;
            Response = response;
        }
        public Result(bool success, T data,HttpStatusCode response)
        {
            Success = success;
            Data = data;
            Response = response;
        }
        public Result(bool success, string message,HttpStatusCode response)
        {
            Success = success;
            Message = message;
            Response = response;
        }
        public Result(bool success,HttpStatusCode response)
        {
            Success = success;
            Response = response;
        }
    }
}