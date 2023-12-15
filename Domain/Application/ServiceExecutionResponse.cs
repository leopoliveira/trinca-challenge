using System;
using System.Net;

namespace Domain.Application
{
    public class ServiceExecutionResponse
    {
        public ServiceExecutionResponse(bool isSuccess, HttpStatusCode httpStatusCode)
        {
            IsSuccess = isSuccess;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(object? data, HttpStatusCode httpStatusCode)
        {
            IsSuccess = data != null;
            Data = data;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(Exception error, HttpStatusCode httpStatusCode)
        {
            IsSuccess = false;
            Error = error;
            Message = error.Message;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(bool isSuccess, string? message, HttpStatusCode httpStatusCode)
        {
            IsSuccess = isSuccess;
            Message = message;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(bool isSuccess, object? data, HttpStatusCode httpStatusCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(string? message, Exception error, HttpStatusCode httpStatusCode)
        {
            IsSuccess = false;
            Message = message;
            Error = error;
            HttpStatusCode = httpStatusCode;
        }

        public ServiceExecutionResponse(bool isSuccess, string? message, object? data, HttpStatusCode httpStatusCode)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            HttpStatusCode = httpStatusCode;
        }

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public Exception? Error { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
