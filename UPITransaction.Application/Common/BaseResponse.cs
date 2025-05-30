namespace UPITransaction.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public BaseResponse(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        //  successful response
        public static BaseResponse<T> SuccessResponse(string message, T data) =>
            new BaseResponse<T>(true, message, data);

        // failed response
        public static BaseResponse<T> FailureResponse(string message) =>
            new BaseResponse<T>(false, message, default);
    }
}
