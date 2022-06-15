using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, badRequest, typeof(ErrorResponse))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, internalServerError, typeof(ErrorResponse))]
    public class BaseController : ControllerBase
    {
        public const string ok = "Success";
        private const string badRequest = "Bad Request";
        private const string internalServerError = "Internal server error";

        public Response<object> Response200 = new()
        {
            Code = 1, // customize
            Message = "Success", // customize
        };

        public Response<object> Response404 = new()
        {
            Code = 0,
            Message = "Not Found",
        };

        public Response<object> Response500 = new()
        {
            Code = 0,
            Message = "Internal server error",
        };

        public class DefaultResponse : Response<object>
        {
            [DataMember(Order = 3)]
            [DefaultValue(null)]
            public override object Data { get; set; }
        }

        public class ErrorResponse : DefaultResponse
        {
            [DataMember(Order = 1)]
            [DefaultValue(0)]
            public override int Code { get; set; }
        }
    }

}
