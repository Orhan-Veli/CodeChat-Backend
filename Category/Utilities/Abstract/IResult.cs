using System;
namespace Category.Utilities.Abstract
{
    public interface IResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }        
        public HttpStatusCode Response { get; set; }
    }
    public enum HttpStatusCode 
    { 
            Ok = 200,
            Created = 201,
            NoContent = 204,
            BadRequest = 400,
            UnAuthorized = 401,
            NotFound = 404,
    }
}