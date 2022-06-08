using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Extensions
{
    public static class DependencyInject
    {
        public static void AddDependencyInject(this IServiceCollection services)
        {
            // services
            services.AddScoped<IMemberService, MemberService>();

            // Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
