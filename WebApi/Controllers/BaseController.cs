using Microsoft.AspNetCore.Mvc;
using Models.Responses;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotAcceptable)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public class BaseController : Controller
    {
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
