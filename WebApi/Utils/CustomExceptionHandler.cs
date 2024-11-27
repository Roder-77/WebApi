using Common.Enums;
using Microsoft.AspNetCore.Diagnostics;
using Models.Exceptions;
using Models.Responses;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WebApi.Utils
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;
        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, $"{nameof(TryHandleAsync)} catch");

            var (statusCode, code, message) = (0, 0, string.Empty);
            object? data = null;

            if (exception is CustomizeException)
            {
                var customizeEx = (exception as CustomizeException)!;
                (statusCode, code, message, data) = ((int)customizeEx.statusCode, (int)customizeEx.code, customizeEx.Message, customizeEx.data);
            }
            else
            {
                (code, message) = ((int)ExceptionCode.Error, exception.Message);
                statusCode = exception switch
                {
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    NotImplementedException => (int)HttpStatusCode.NotImplemented,
                    _ => (int)HttpStatusCode.InternalServerError,
                };
            }

            var response = JsonSerializer.Serialize(
                new Response<object?> { Code = code, Message = message, Data = data },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(response, cancellationToken);

            return false;
        }
    }
}
