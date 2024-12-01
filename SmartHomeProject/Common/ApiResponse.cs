using System.Collections.Generic;

namespace SmartHomeProject.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResponse<T> Error(List<string> errors, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}