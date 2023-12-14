using Common.Enums;
using System.Net;

namespace Models.Exceptions
{
    public class ForbiddenException : CustomizeException
    {
        private const HttpStatusCode _statusCode = HttpStatusCode.Forbidden;

        public ForbiddenException(string message) : base(_statusCode, message)
        { }

        public ForbiddenException(string message, ExceptionCode? code = null) : base(_statusCode, message, code)
        { }

        public ForbiddenException(string message, object? data = null, ExceptionCode? code = null) : base(_statusCode, message, data, code)
        { }
    }
}
