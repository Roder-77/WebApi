using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    public static class ActionFilterExtension
    {
        public static bool HasActionAttribute<T>(this ActionExecutingContext context) where T : Attribute
            => context.ActionDescriptor.EndpointMetadata.Any(x => x.GetType() == typeof(T));
    }
}
