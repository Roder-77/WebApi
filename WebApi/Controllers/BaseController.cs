using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Responses;
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
    [SwaggerResponse((int)HttpStatusCode.NotAcceptable, notAcceptable, typeof(ErrorResponse))]
    [SwaggerResponse((int)HttpStatusCode.UnprocessableEntity, unprocessableEntity, typeof(ErrorResponse))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, internalServerError, typeof(ErrorResponse))]
    public class BaseController : Controller
    {
        private const string badRequest = "Bad Request";
        private const string unauthorized = "Unauthorized";
        private const string notAcceptable = "Not Acceptable";
        private const string unprocessableEntity = "Unprocessable Entity";
        private const string internalServerError = "Internal Server Error";

        protected Response<object> Response200 = new() { Code = 1, Message = "Success" };

        protected Response<object> Response404 = new() { Code = 0, Message = "Not Found" };

        protected Response<object> Response500 = new() { Code = 0, Message = "Internal Server Error" };

        protected class ErrorResponse : DefaultResponse
        {
            [DataMember(Order = 1)]
            [DefaultValue(0)]
            public override int Code { get; set; }
        }
    }

}
