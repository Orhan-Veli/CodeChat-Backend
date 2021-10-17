using System;
namespace Message.Utilities.Abstract
{
    public interface IResult<T>
    {
        bool Success { get; set; }
        string Message { get; set; }
        T Data { get; set; }
        HttpStatusCode Response { get; set; }
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