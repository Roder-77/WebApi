using Common.Enums;
using System.Net;

namespace Models.Exceptions
{
    public class UnprocessableEntityException : CustomizeException
    {
        private const HttpStatusCode _statusCode = HttpStatusCode.UnprocessableEntity;

        public UnprocessableEntityException(string message) : base(_statusCode, message)
        { }

        public UnprocessableEntityException(string message, ExceptionCode? code = null) : base(_statusCode, message, code)
        { }

        public UnprocessableEntityException(string message, object? data = null, ExceptionCode? code = null) : base(_statusCode, message, data, code)
        { }
    }
}
