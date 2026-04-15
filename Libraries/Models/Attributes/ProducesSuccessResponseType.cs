using Microsoft.AspNetCore.Mvc;
using Models.Responses;
using System.Net;

namespace Models.Attributes
{
    public class ProducesSuccessResponseType : ProducesResponseTypeAttribute
    {
        public ProducesSuccessResponseType(int statusCode) : base(statusCode)
        { }

        public ProducesSuccessResponseType(Type? type = null) : base(type ?? typeof(DefaultResponse), (int)HttpStatusCode.OK)
        { }

        public ProducesSuccessResponseType(Type? type = null, string contentType = "application/json", params string[] additionalContentTypes) : base(type ?? typeof(DefaultResponse), (int)HttpStatusCode.OK, contentType, additionalContentTypes)
        { }
    }
}
