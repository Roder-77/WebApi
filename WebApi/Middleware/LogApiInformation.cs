using Microsoft.AspNetCore.Http.Extensions;
using Models.Response;
using System.Net;
using System.Text.Json;

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

            _logger.LogInformation($"HttpMethod: {request.Method}, Url: {url}, Body: {body}");
        }

        private async Task LogResponse(HttpContext context, Exception ex)
        {
            var (statusCode, code, message) = ex switch
            {
                // add costomize exception
                NullReferenceException => ((int)HttpStatusCode.NotFound, 0, ""),
                _ => ((int)HttpStatusCode.InternalServerError, 0, ex.Message),
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = JsonSerializer.Serialize(new Response<object> { Code = code, Message = message });
            await context.Response.WriteAsync(response);

            _logger.LogError(ex, $"{nameof(LogResponse)} catch");
        }
    }
}
