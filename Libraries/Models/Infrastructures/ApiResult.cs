using Microsoft.AspNetCore.Http;
using System.Net;

namespace Models.Infrastructures
{
    public class ApiResult<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 回應資料
        /// </summary>
        public TResponse? Response { get; set; }
    }

    /// <summary>
    /// API 呼叫結果包裝類
    /// </summary>
    /// <typeparam name="TResponse">成功回應類型</typeparam>
    /// <typeparam name="TErrorResponse">錯誤回應類型</typeparam>
    public class ApiResult<TResponse, TErrorResponse> : ApiResult<TResponse>
        where TResponse : class
        where TErrorResponse : class
    {
        /// <summary>
        /// 錯誤回應資料
        /// </summary>
        public TErrorResponse? ErrorResponse { get; set; }
    }
}
