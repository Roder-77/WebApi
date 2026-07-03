using Common.Enums;
using System.Net;

namespace Models.Exceptions
{
    public class CustomizeException : Exception
    {
        private readonly HttpStatusCode statusCode;
        private readonly ExceptionCode code;
        private readonly object? data;

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

        public (int statusCode, int code, string message, object? data) ToTuple()
        {
            return ((int)statusCode, (int)code, Message, data);
        }
    }
}