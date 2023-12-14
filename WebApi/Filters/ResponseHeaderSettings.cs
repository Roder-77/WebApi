using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace WebApi.Filters
{
    public class ResponseHeaderSettings : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(HeaderNames.AccessControlExposeHeaders, HeaderNames.ContentDisposition);
        }
    }
}
