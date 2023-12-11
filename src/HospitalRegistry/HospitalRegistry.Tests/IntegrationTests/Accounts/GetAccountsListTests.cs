using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalRegistry.Tests.IntegrationTests.Accounts;

public class GetAccountsListTests : IntegrationTestsBase
{
    public GetAccountsListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_AccountsList_RoleIsNotPassed_ReturnsBadRequest()
    {
        // Act
        var response = await client.GetAsync($"api/v1/accounts");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_AccountsList_IncorrectSpecifications_ReturnsBadRequest()
    {
        // Assert
        int pageNumber = -1;
        int pageSize = -1;
        string role = "Admin";

        var scope = factory.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roleToCreate = new ApplicationRole
        {
            Name = role
        };

        await roleManager.CreateAsync(roleToCreate);

        // Act
        var response = await client.GetAsync($"api/v1/accounts?role={role}&pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_AccountsList_ValidSpecifications_ReturnsOK()
    {
        // Assert
        int pageNumber = 1;
        int pageSize = 10;
        string role = "Admin";

        var scope = factory.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roleToCreate = new ApplicationRole
        {
            Name = role
        };

        await roleManager.CreateAsync(roleToCreate);

        // Act
        var response = await client.GetAsync($"api/v1/accounts?role={role}&pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
