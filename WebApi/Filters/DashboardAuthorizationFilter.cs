using Hangfire.Annotations;
using Hangfire.Dashboard;
using Services.Extensions;

namespace WebApi.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext is null)
                return false;

            var account = httpContext.GetMember();
            if (account is null)
                return false;

            // ToDo
            return true;
        }
    }
}
