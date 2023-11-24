using HospitalRegistry.Infrastructure.DatabaseInitializer;
using HospitalRegistry.Infrastructure.Repositories;
using HospitalReqistry.Application.RepositoryContracts;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalRegistry.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAsyncRepository, RepositoryBase>();
            services.AddTransient<IDatabaseInitializer, DbInitializer>();

            return services;
        }
    }
}
