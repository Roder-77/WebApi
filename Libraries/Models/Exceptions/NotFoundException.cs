using Common.Enums;
using System.Net;

namespace Models.Exceptions
{
    public class NotFoundException : CustomizeException
    {
        private const HttpStatusCode _statusCode = HttpStatusCode.NotFound;

        public NotFoundException(string message) : base(_statusCode, message)
        { }

        public NotFoundException(string message, ExceptionCode? code = null) : base(_statusCode, message, code)
        { }

        public NotFoundException(string message, object? data = null, ExceptionCode? code = null) : base(_statusCode, message, data, code)
        { }
    }
}
