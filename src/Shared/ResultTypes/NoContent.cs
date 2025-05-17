using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.ResultTypes
{
    public class NoContent { }

    public class Response<T>
    {
        public T Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public int StatusCode { get; private set; }
        public IReadOnlyList<string> Errors { get; private set; }

        private Response() { }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { 
                Data = data,
                IsSuccess = true,
                StatusCode = statusCode 
            };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T>
            {
                IsSuccess = true,
                StatusCode = statusCode
            };
        }
        public static Response<T> Fail(IEnumerable<string> errors, int statusCode)
        {
            return new Response<T>
            {
                IsSuccess = false,
                Errors = new List<string>(errors),
                StatusCode = statusCode
            };
        }

        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T>
            {
                IsSuccess = false,
                Errors = new List<string> { error },
                StatusCode = statusCode
            };
        }

    }
}
