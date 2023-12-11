using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Doctors;

public class CreateDoctorTests : IntegrationTestsBase
{
    public CreateDoctorTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task POST_CreateDoctor_NullPassed_ReturnsBadRequest()
    {
        // Arrange
        DoctorAddRequest request = null;

        // Act
        var response = await client.PostAsJsonAsync("api/v1/doctors", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_CreateDoctor_ValidObject_ReturnsOK()
    {
        // Arrange
        DoctorAddRequest request = fixture.Build<DoctorAddRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .With(x => x.Email, "testEmail@gmail.com")
            .With(x => x.PhoneNumber, "+380959999999")
            .Create();

        // Act
        var response = await client.PostAsJsonAsync("api/v1/doctors", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
