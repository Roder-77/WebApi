using Microsoft.Extensions.Logging;
using Models.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services.Helpers
{
    public class CallApiHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CallApiHelper> _logger;

        public CallApiHelper(ILogger<CallApiHelper> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 加入基本 headers
        /// </summary>
        /// <param name="headers"></param>
        private void AddDefaultHeaders(HttpRequestHeaders headers)
        {
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", "");
        }

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <param name="httpMethod">http method</param>
        /// <param name="requestUri">request uri</param>
        /// <param name="body">body</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public async Task<(bool, TResponse?)> CallAPI<TResponse>(HttpMethod httpMethod, string requestUri, object? body = null, Dictionary<string, string>? headers = null)
            where TResponse : class
        {
            var requestInfo = $"request httpMethod: {httpMethod}, requestUri: {requestUri}, body: {JsonSerializer.Serialize(body)}";

            try
            {
                using var request = new HttpRequestMessage(httpMethod, requestUri);

                if (headers != null)
                {
                    AddDefaultHeaders(request.Headers);

                    foreach (var header in headers)
                        request.Headers.Add(header.Key, header.Value);
                }

                if (httpMethod.HasBody() && body != null)
                {
                    var json = JsonSerializer.Serialize(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var client = _httpClientFactory.CreateClient(nameof(CallAPI));
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseInfo = $"response statusCode: {(int)response.StatusCode}, content: {content}";
                    _logger.LogError($"{nameof(CallAPI)} fail, {requestInfo}{Environment.NewLine}{responseInfo}");
                }

                if (!string.IsNullOrWhiteSpace(content))
                    return (response.IsSuccessStatusCode, JsonSerializer.Deserialize<TResponse>(content));

                return (response.IsSuccessStatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CallAPI)} error, {requestInfo}");
                throw;
            }
        }
    }
}
