using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public static BaseResponse Success() => new BaseResponse { IsSuccess = true };
        public static BaseResponse Failure(string error) => new BaseResponse { IsSuccess = false, ErrorMessage = error };
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }

        public static BaseResponse<T> Success(T data) => new BaseResponse<T> { IsSuccess = true, Data = data };
        public new static BaseResponse<T> Failure(string error) => new BaseResponse<T> { IsSuccess = false, ErrorMessage = error };
    }
}
