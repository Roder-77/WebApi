using Common.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Models.Exceptions;
using Models.Infrastructures;
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
        private readonly HashSet<string> _sensitiveFields = new(StringComparer.OrdinalIgnoreCase) { "password", "creditCard" };

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
            var url = request.GetDisplayUrl();
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

                var jsonOptions = new JsonSerializerOptions(GlobalSettings.JsonOptions) { WriteIndented = true };
                body = JsonSerializer.Serialize(formDict, jsonOptions);
            }

            _logger.LogInformation("HttpMethod: {HttpMethod}, Url: {Url}, Body: {Body}", request.Method, url, body);
        }

        private async Task LogResponse(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "{MethodName} catch", nameof(LogResponse));

            var (statusCode, code, message) = (0, 0, string.Empty);
            object? data = null;

            if (ex is CustomizeException customizeEx)
            {
                (statusCode, code, message, data) = customizeEx.ToTuple();
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

            //if (context.GetEndpoint()?.Metadata.GetMetadata<ApiControllerAttribute>() == null)
            //{
            //    context.Response.Redirect($"/Error?code={code}&message={message}");
            //    return;
            //}

            var response = JsonSerializer.Serialize(new Response<object?> { Code = code, Message = message, Data = data }, GlobalSettings.JsonOptions);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(response);
        }
    }
}