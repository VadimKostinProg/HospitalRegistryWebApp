using HospitalRegistry.Application.Initializers;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalRegistry.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDoctorsService, DoctorsService>();
            services.AddScoped<IPatientsService, PatientsService>();
            services.AddScoped<IDiagnosesService, DiagnosesService>();
            services.AddScoped<ISchedulesService, SchedulesService>();
            services.AddScoped<IAppointmentsService, AppointmentsService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserAccountsService, UserAccountsService>();
            services.AddTransient<IUserInitializer, UserInitializer>();

            return services;
        }
    }
}
