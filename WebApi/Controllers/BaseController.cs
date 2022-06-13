using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, badRequest, typeof(Response<object>))]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, internalServerError, typeof(Response<object>))]
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
    }
}
