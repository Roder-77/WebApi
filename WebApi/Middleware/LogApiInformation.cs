using Common.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Models.Exceptions;
using Models.Responses;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WebApi.Middleware
{
    public class LogApiInformation
    {
        private readonly ILogger<LogApiInformation> _logger;
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        private readonly HashSet<string> _sensitiveFields = new(StringComparer.OrdinalIgnoreCase){ "password", "creditCard" };

        public LogApiInformation(ILogger<LogApiInformation> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await LogRequest(context);
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogResponse(context, ex);
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            var request = context.Request;
            var url = UriHelper.GetDisplayUrl(request);
            var body = string.Empty;

            if (!request.HasFormContentType)
            {
                request.EnableBuffering();
                var reader = new StreamReader(request.Body, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }
            else
            {
                var form = await request.ReadFormAsync();
                var formDict = form.ToDictionary(
                    k => k.Key,
                    v => _sensitiveFields.Contains(v.Key) ? "********" : v.Value.Count > 1 ? (object)v.Value.ToList() : v.Value.ToString()
                );
                
                foreach (var file in form.Files)
                    formDict[file.Name] = new { file.FileName, file.Length };

                body = JsonSerializer.Serialize(formDict, _jsonSerializerOptions);
            }
            
            _logger.LogInformation($"HttpMethod: {request.Method}, Url: {url}, Body: {body}");
        }

        private async Task LogResponse(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LogResponse)} catch");

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

            var response = JsonSerializer.Serialize(new Response<object?> { Code = code, Message = message, Data = data }, _jsonSerializerOptions);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(response);
        }
    }
}
