using Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Models.Attributes
{
    public class SwaggerSuccessResponse : SwaggerResponseAttribute
    {
        private const int _statusCode200 = (int)HttpStatusCode.OK;
        private const string _ok = "Success";

        public SwaggerSuccessResponse(Type? type = null) : base(_statusCode200, _ok, type ?? typeof(DefaultResponse))
        { }

        public SwaggerSuccessResponse(Type? type = null, params string[] contentTypes) : base(_statusCode200, _ok, type ?? typeof(DefaultResponse), contentTypes)
        { }
    }
}
