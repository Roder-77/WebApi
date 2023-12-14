using Common.Enums;
using System.Net;

namespace Models.Exceptions
{
    public class CustomizeException : Exception
    {
        public HttpStatusCode statusCode;
        public ExceptionCode code;
        public object? data;

        public CustomizeException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.statusCode = statusCode;
            code = ExceptionCode.Error;
        }

        public CustomizeException(HttpStatusCode statusCode, string message, ExceptionCode? code = null) : base(message)
        {
            this.statusCode = statusCode;
            this.code = code ?? ExceptionCode.Error;
        }

        public CustomizeException(HttpStatusCode statusCode, string message, object? data = null, ExceptionCode? code = null) : base(message)
        {
            this.statusCode = statusCode;
            this.code = code ?? ExceptionCode.Error;
            this.data = data;
        }
    }
}
