using Microsoft.Extensions.Logging;
using Models.Infrastructures;
using Services.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services.Infrastructures
{
    public class CallApiService
    {
        private static string _jsonMediaType = "application/json";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CallApiService> _logger;

        public CallApiService(ILogger<CallApiService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        #region Utilities

        /// <summary>
        /// 加入基本 headers
        /// </summary>
        /// <param name="headers"></param>
        private void AddDefaultHeaders(HttpRequestHeaders headers)
        {
            headers.Accept.Add(new(_jsonMediaType));
            //headers.Authorization = new("");
        }

        private StringContent? GenerateBody(object? body, JsonSerializerOptions? jsonOptions = null)
        {
            if (body is null)
                return null;

            var json = JsonSerializer.Serialize(body, jsonOptions);
            return new StringContent(json, Encoding.UTF8, _jsonMediaType);
        }

        #endregion

        public async Task<ApiResult<TResponse>> Get<TResponse>(string requestUri, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Get, requestUri, null, headers);

        public async Task<ApiResult<TResponse>> Post<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Post, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse>> Post<TResponse>(string requestUri, IEnumerable<KeyValuePair<string, string>> formData, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Post, requestUri, new FormUrlEncodedContent(formData), headers);

        public async Task<ApiResult<TResponse>> Put<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Put, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse>> Patch<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Patch, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse>> Delete<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse, TResponse>(HttpMethod.Delete, requestUri, GenerateBody(body, jsonOptions), headers);

        #region 指定錯誤回應類型的 API 呼叫

        public async Task<ApiResult<TResponse, TErrorResponse>> GetWithError<TResponse, TErrorResponse>(string requestUri, Dictionary<string, string>? headers = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Get, requestUri, null, headers);

        public async Task<ApiResult<TResponse, TErrorResponse>> PostWithError<TResponse, TErrorResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Post, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse, TErrorResponse>> PostWithError<TResponse, TErrorResponse>(string requestUri, IEnumerable<KeyValuePair<string, string>> formData, Dictionary<string, string>? headers = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Post, requestUri, new FormUrlEncodedContent(formData), headers);

        public async Task<ApiResult<TResponse, TErrorResponse>> PutWithError<TResponse, TErrorResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Put, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse, TErrorResponse>> PatchWithError<TResponse, TErrorResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Patch, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<ApiResult<TResponse, TErrorResponse>> DeleteWithError<TResponse, TErrorResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            where TErrorResponse : class
            => await CallAPI<TResponse, TErrorResponse>(HttpMethod.Delete, requestUri, GenerateBody(body, jsonOptions), headers);

        #endregion

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <typeparam name="TResponse">回傳結果</typeparam>
        /// <typeparam name="TErrorResponse">错误回傳結果</typeparam>
        /// <param name="httpMethod">http method</param>
        /// <param name="requestUri">request uri</param>
        /// <param name="httpContent">httpContent</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public async Task<ApiResult<TResponse, TErrorResponse>> CallAPI<TResponse, TErrorResponse>(HttpMethod httpMethod, string requestUri, HttpContent? httpContent, Dictionary<string, string>? headers = null)
            where TResponse : class
            where TErrorResponse : class
        {
            var methodName = nameof(CallAPI);
            var body = httpContent is null ? string.Empty : await httpContent.ReadAsStringAsync();
            var requestInfo = $"request httpMethod: {httpMethod}, requestUri: {requestUri}, {Environment.NewLine}body: {body}";

            _logger.LogInformation("{MethodName} start, {RequestInfo:l}", methodName, requestInfo);

            try
            {
                using var request = new HttpRequestMessage(httpMethod, requestUri);

                AddDefaultHeaders(request.Headers);

                if (headers != null)
                {
                    foreach (var header in headers)
                        request.Headers.Add(header.Key, header.Value);
                }

                if (httpContent != null)
                    request.Content = httpContent;

                var client = _httpClientFactory.CreateClient(nameof(CallAPI));
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("{MethodName} end, response status code: {StatusCode}, content: {Content:l}", methodName, (int)response.StatusCode, content);



                var result = new ApiResult<TResponse, TErrorResponse>
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                };

                if (string.IsNullOrWhiteSpace(content))
                {
                    return result;
                }

                if (response.IsSuccessStatusCode)
                {
                    result.Response = content.FromJson<TResponse>();
                }
                else
                {
                    result.ErrorResponse = content.FromJson<TErrorResponse>();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{MethodName} error", methodName);
                throw;
            }
        }
    }
}
