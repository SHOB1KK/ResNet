namespace Domain.Responses;
using System;
using System.Net;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public Response() { }  // пустой конструктор для сериализации

    public Response(T? data)
    {
        Data = data;
        IsSuccess = true;
        StatusCode = 200;
        Message = null;
    }

    public Response(HttpStatusCode statusCode, string message)
    {
        IsSuccess = false;
        Data = default;
        StatusCode = (int)statusCode;
        Message = message;
    }

    public static Response<T> Success(T data)
    {
        return new Response<T>
        {
            Data = data,
            IsSuccess = true,
            StatusCode = (int)HttpStatusCode.OK,
            Message = null
        };
    }
    
    public static Response<T> Success(T data, string message)
    {
        return new Response<T>
        {
            Data = data,
            Message = message,
            IsSuccess = true,
            StatusCode = (int)HttpStatusCode.OK
        };
    }
}
