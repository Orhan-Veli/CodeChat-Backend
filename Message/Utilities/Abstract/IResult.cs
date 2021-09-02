using System;
namespace Message.Utilities.Abstract
{
    public interface IResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}