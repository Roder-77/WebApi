using Microsoft.Extensions.Logging;
using Models.Responses;
using Services.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services
{
    public class CallApiService
    {
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
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", "");
        }

        private StringContent GenerateBody(object body)
        {
            var json = JsonSerializer.Serialize(body);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        #endregion

        public async Task<(bool, TResponse?)> Get<TResponse>(string requestUri, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Get, requestUri, null, headers);

        public async Task<(bool, TResponse?)> Post<TRequest, TResponse>(string requestUri, TRequest body, Dictionary<string, string>? headers = null)
            where TRequest : class
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Post, requestUri, GenerateBody(body), headers);

        public async Task<(bool, TResponse?)> Post<TResponse>(string requestUri, IEnumerable<KeyValuePair<string, string>> formData, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Post, requestUri, new FormUrlEncodedContent(formData), headers);

        public async Task<(bool, TResponse?)> Put<TResponse>(string requestUri, object body, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Put, requestUri, GenerateBody(body), headers);

        public async Task<(bool, TResponse?)> Patch<TResponse>(string requestUri, object body, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Patch, requestUri, GenerateBody(body), headers);

        public async Task<(bool, TResponse?)> Delete<TResponse>(string requestUri, object body, Dictionary<string, string>? headers = null)
            where TResponse : class
            => await CallAPI<TResponse>(HttpMethod.Delete, requestUri, GenerateBody(body), headers);

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <param name="httpMethod">http method</param>
        /// <param name="requestUri">request uri</param>
        /// <param name="httpContent">httpContent</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public async Task<(bool, TResponse?)> CallAPI<TResponse>(HttpMethod httpMethod, string requestUri, HttpContent? httpContent, Dictionary<string, string>? headers = null)
            where TResponse : class
        {
            var requestInfo = $"request httpMethod: {httpMethod}, requestUri: {requestUri}, body: {await httpContent.ReadAsStringAsync()}";

            try
            {
                using var request = new HttpRequestMessage(httpMethod, requestUri);

                if (headers != null)
                {
                    AddDefaultHeaders(request.Headers);

                    foreach (var header in headers)
                        request.Headers.Add(header.Key, header.Value);
                }

                if (httpContent != null)
                    request.Content = httpContent;

                var client = _httpClientFactory.CreateClient(nameof(CallAPI));
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseInfo = $"response statusCode: {(int)response.StatusCode}, content: {content}";
                    _logger.LogError($"{nameof(CallAPI)} fail, {requestInfo}{Environment.NewLine}{responseInfo}");
                }

                if (string.IsNullOrWhiteSpace(content))
                    return (response.IsSuccessStatusCode, default);

                if (typeof(TResponse) == typeof(Response<string>))
                    return (response.IsSuccessStatusCode, new Response<string> { Data = content } as TResponse);

                return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CallAPI)} error, {requestInfo}");
                throw;
            }
        }
    }
}
