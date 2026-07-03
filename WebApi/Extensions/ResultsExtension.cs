using Models.Responses;
using System.Net;

namespace WebApi.Extensions
{
    public class APiResults : IResult
    {
        private readonly Response<object> _response;
        private readonly int _statusCode;

        public APiResults(HttpStatusCode statusCode = HttpStatusCode.OK, int code = 1, string message = "Success", object data = null!)
        {
            _statusCode = (int)statusCode;
            _response = new() { Code = code, Message = message, Data = data };
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = _statusCode;
            return httpContext.Response.WriteAsJsonAsync(_response);
        }

        public static APiResults Ok(object data) => new(data: data);

        public static APiResults Error400(string message = "Bad Request", object data = null!) => new(HttpStatusCode.BadRequest, 0, message, data);

        public static APiResults Error404(string message = "Not Found", object data = null!) => new(HttpStatusCode.NotFound, 0, message, data);

        public static APiResults Error422(string message = "Unprocessable Entity", object data = null!) => new(HttpStatusCode.UnprocessableEntity, 0, message, data);

        public static APiResults Error500(string message = "Internal Server Error", object data = null!) => new(HttpStatusCode.InternalServerError, 0, message, data);

        public static APiResults Custom(HttpStatusCode statusCode, int code, string message, object data = null!) => new(statusCode, code, message, data);
    }
}