using Microsoft.Extensions.Logging;
using Models.Responses;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services
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

        public async Task<(bool IsSuccess, TResponse? Response)> Get<TResponse>(string requestUri, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Get, requestUri, null, headers);

        public async Task<(bool IsSuccess, TResponse? Response)> Post<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Post, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<(bool IsSuccess, TResponse? Response)> Post<TResponse>(string requestUri, IEnumerable<KeyValuePair<string, string>> formData, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Post, requestUri, new FormUrlEncodedContent(formData), headers);

        public async Task<(bool IsSuccess, TResponse? Response)> Put<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Put, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<(bool IsSuccess, TResponse? Response)> Patch<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Patch, requestUri, GenerateBody(body, jsonOptions), headers);

        public async Task<(bool IsSuccess, TResponse? Response)> Delete<TResponse>(string requestUri, object? body = null, Dictionary<string, string>? headers = null, JsonSerializerOptions? jsonOptions = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Delete, requestUri, GenerateBody(body, jsonOptions), headers);

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <param name="httpMethod">http method</param>
        /// <param name="requestUri">request uri</param>
        /// <param name="httpContent">httpContent</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, TResponse? Response)> CallAPI<TResponse>(HttpMethod httpMethod, string requestUri, HttpContent? httpContent, Dictionary<string, string>? headers = null)
            where TResponse : class
        {
            var methodName = nameof(CallAPI);
            var body = httpContent is null ? string.Empty : await httpContent.ReadAsStringAsync();
            var requestInfo = $"request httpMethod: {httpMethod}, requestUri: {requestUri}, {Environment.NewLine}body: {body}";

            _logger.LogInformation($"{methodName} start, {requestInfo}");

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
                var responseInfo = $"response status code: {(int)response.StatusCode}, content: {content}";

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"{methodName} fail, response: {responseInfo}");
                    return (response.IsSuccessStatusCode, default);
                }

                _logger.LogInformation($"{methodName} end, response: {responseInfo}");

                if (string.IsNullOrWhiteSpace(content))
                    return (response.IsSuccessStatusCode, default);

                if (typeof(TResponse) == typeof(Response<string>))
                    return (response.IsSuccessStatusCode, new Response<string> { Data = content } as TResponse);

                return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{methodName} error");
                throw;
            }
        }
    }
}
