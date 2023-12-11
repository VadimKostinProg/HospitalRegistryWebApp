using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Accounts;

public class LoginTests : IntegrationTestsBase
{
    public LoginTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task POST_Login_NullPassed_ReturnsBadRequest()
    {
        // Arrange
        LoginDTO request = null;

        // Act
        var response = await client.PostAsJsonAsync("api/v1/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Login_IncorrectEmail_ReturnsNotFound()
    {
        // Arrange
        LoginDTO request = new()
        {
            Email = "testEmail@gmail.com",
            Password = "password12345!"
        };

        // Act
        var response = await client.PostAsJsonAsync("api/v1/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_Login_IncorrectPassword_ReturnsBadRequest()
    {
        // Arrange
        var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string email = "testEmail@gmail.com";
        string password = "password12345!";

        ApplicationUser userToCreate = new()
        {
            UserName = email,
            FullName = "Test name",
            Email = email,
        };

        await userManager.CreateAsync(userToCreate, password);

        LoginDTO request = new()
        {
            Email = email,
            Password = password + "123"
        };

        // Act
        var response = await client.PostAsJsonAsync("api/v1/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Login_ValidCredentials_ReturnsOK()
    {
        // Arrange
        var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var email = "testEmail@gmail.com";
        var password = "password12345!";

        ApplicationUser userToCreate = new()
        {
            UserName = email,
            FullName = "Test name",
            Email = email,
        };

        ApplicationRole adminRole = new ApplicationRole
        {
            Name = "Admin"
        };

        var result = await userManager.CreateAsync(userToCreate, password);
        await roleManager.CreateAsync(adminRole);
        await userManager.AddToRoleAsync(userToCreate, "Admin");

        LoginDTO request = new()
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await client.PostAsJsonAsync("api/v1/accounts/login", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
