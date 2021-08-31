using System;
namespace Category.Utilities.Abstract
{
    public interface IResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}