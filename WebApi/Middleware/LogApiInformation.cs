using Common.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Models.Exceptions;
using Models.Response;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebApi.Middleware
{
    public class LogApiInformation
    {
        private readonly ILogger<LogApiInformation> _logger;
        private readonly RequestDelegate _next;

        public LogApiInformation(ILogger<LogApiInformation> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var request = context.Request;

                await LogRequest(request);
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogResponse(context, ex);
            }
        }

        private async Task LogRequest(HttpRequest request)
        {
            var url = UriHelper.GetDisplayUrl(request);
            var cloneBody = new MemoryStream();

            await request.Body.CopyToAsync(cloneBody);
            cloneBody.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(cloneBody);
            var body = await reader.ReadToEndAsync();
            cloneBody.Seek(0, SeekOrigin.Begin);
            request.Body = cloneBody;

            if (!body.Contains("form-data"))
            {
                _logger.LogInformation($"HttpMethod: {request.Method}, Url: {url}, Body: {body}");
                return;
            }

            var fileNames = Regex.Matches(body, "(?<=filename=\").*(?=\")", RegexOptions.Multiline)
                    .Select(x => x.Value);

            _logger.LogInformation($"HttpMethod: {request.Method}, Url: {url}, file name: {string.Join(", ", fileNames)}");
        }

        private async Task LogResponse(HttpContext context, Exception ex)
        {
            var (statusCode, code, message) = (0, 0, string.Empty);
            object? data = null;

            if (ex is CustomizeException)
            {
                var customizeEx = (ex as CustomizeException)!;
                (statusCode, code, message, data) = ((int)customizeEx.statusCode, (int)customizeEx.code, customizeEx.Message, customizeEx.data);
            }
            else
            {
                (code, message) = ((int)ExceptionCode.Error, ex.Message);
                statusCode = ex switch
                {
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    NotImplementedException => (int)HttpStatusCode.NotImplemented,
                    _ => (int)HttpStatusCode.InternalServerError,
                };
            }

            var response = JsonSerializer.Serialize(
                new Response<object> { Code = code, Message = message, Data = data },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(response);

            _logger.LogError(ex, $"{nameof(LogResponse)} catch");
        }
    }
}
