using System;

namespace Domain.Application
{
    public class ServiceExecutionResponse
    {
        public ServiceExecutionResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public ServiceExecutionResponse(object? data)
        {
            IsSuccess = data != null;
            Data = data;
        }

        public ServiceExecutionResponse(Exception error)
        {
            IsSuccess = false;
            Error = error;
            Message = error.Message;
        }

        public ServiceExecutionResponse(bool isSuccess, string? message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public ServiceExecutionResponse(bool isSuccess, object? data)
        {
            IsSuccess = isSuccess;
            Data = data;
        }

        public ServiceExecutionResponse(string? message, Exception error)
        {
            IsSuccess = false;
            Message = message;
            Error = error;
        }

        public ServiceExecutionResponse(bool isSuccess, string? message, object? data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public Exception? Error { get; set; }
    }
}
