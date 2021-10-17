using System;
using Category.Utilities.Abstract;
namespace Category.Utilities.Concrete
{
    public class Result<T> : IResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
        public HttpStatusCode Response { get; set; }

        public Result(string message, bool success, T data,HttpStatusCode response)
        {
            Success = success;
            Message = message;
            Data = data;
            Response = response;
        }
        public Result(bool success, string message,HttpStatusCode response)
        {
            Success = success;
            Message = message;
            Response = response;
        }
        public Result(bool success, T data,HttpStatusCode response)
        {
            Data = data;
            Success = success;
            Response = response;
        }
        public Result(bool success,HttpStatusCode response)
        {
            Success = success;
            Response = response;
        }
    }
}