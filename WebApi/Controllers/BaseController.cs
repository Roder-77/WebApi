using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, badRequest, typeof(ErrorResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, unauthorized, typeof(ErrorResponse))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, internalServerError, typeof(ErrorResponse))]
    public class BaseController : ControllerBase
    {
        protected const int statusCode200 = (int)HttpStatusCode.OK;
        protected const string ok = "Success";
        private const string badRequest = "Bad request";
        private const string unauthorized = "Unauthorized";
        private const string internalServerError = "Internal server error";

        protected Response<object> Response200 = new()
        {
            Code = 1, // customize
            Message = "Success", // customize
        };

        protected Response<object> Response404 = new()
        {
            Code = 0,
            Message = "Not Found",
        };

        protected Response<object> Response500 = new()
        {
            Code = 0,
            Message = "Internal server error",
        };

        protected class DefaultResponse : Response<object>
        {
            [DataMember(Order = 3)]
            [DefaultValue(null)]
            public override object Data { get; set; }
        }

        protected class ErrorResponse : DefaultResponse
        {
            [DataMember(Order = 1)]
            [DefaultValue(0)]
            public override int Code { get; set; }
        }
    }

}
