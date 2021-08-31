using System;
using Category.Utilities.Abstract;
namespace Category.Utilities.Concrete
{
    public class Result<T> : IResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }

        public Result(string message, bool success, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public Result(bool success, T data)
        {
            Data = data;
            Success = success;
        }
        public Result(bool success)
        {
            Success = success;
        }
    }
}