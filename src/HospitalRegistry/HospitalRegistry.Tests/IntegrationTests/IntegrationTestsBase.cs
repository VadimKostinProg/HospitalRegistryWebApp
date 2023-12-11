using HospitalRegistry.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalRegistry.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase : HospitalRegistryTestsBase, IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> factory;
        protected readonly ApplicationContext context;
        protected readonly HttpClient client;
        protected readonly IConfiguration configuration;

        public IntegrationTestsBase(CustomWebApplicationFactory<Program> factory)
        {
            this.factory = factory;

            var scope = this.factory.Services.CreateScope();

            this.context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            this.configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            this.client = this.factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            this.client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", 
                    this.GenerateTestToken(this.configuration));
        }
    }
}
