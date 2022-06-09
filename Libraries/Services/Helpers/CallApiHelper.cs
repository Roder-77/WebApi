using Microsoft.Extensions.Logging;
using Models.Extensions;
using System.Text;
using System.Text.Json;

#nullable disable

namespace Service.Helpers
{
    public class CallApiHelper
    {
        private readonly HttpClient _client;
        private readonly ILogger<CallApiHelper> _logger;

        public CallApiHelper(ILogger<CallApiHelper> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _client = clientFactory.CreateClient(nameof(CallApiHelper));
        }

        /// <summary>
        /// 呼叫 API
        /// </summary>
        /// <param name="httpMethod">http method</param>
        /// <param name="requestUri">request uri</param>
        /// <param name="body">body</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public async Task<TResponse> CallAPI<TRequest, TResponse>(HttpMethod httpMethod, string requestUri, TRequest body, Dictionary<string, string> headers = null)
            where TRequest : class
            where TResponse : class
        {
            try
            {
                using (var request = new HttpRequestMessage(httpMethod, requestUri))
                {
                    if (headers != null)
                    {
                        foreach (var header in headers)
                            request.Headers.Add(header.Key, header.Value);
                    }

                    if (httpMethod.HasBody() && body != null)
                    {
                        var json = JsonSerializer.Serialize(body);
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    }

                    var response = await _client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return JsonSerializer.Deserialize<TResponse>(content);

                    if (!string.IsNullOrEmpty(content))
                        return JsonSerializer.Deserialize<TResponse>(content);

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CallAPI)} fail, httpMethod: {httpMethod}, requestUri: {requestUri}, body: {JsonSerializer.Serialize(body)}");
                throw;
            }
        }

    }
}
