using Microsoft.AspNetCore.Http;
using Models.DataModels;

namespace Services.Extensions
{
    public static class HttpContextExtension
    {
        public static Member GetMember(this HttpContext httpContext)
        {
            return (Member)httpContext.Items["Member"];
        }
    }
}
