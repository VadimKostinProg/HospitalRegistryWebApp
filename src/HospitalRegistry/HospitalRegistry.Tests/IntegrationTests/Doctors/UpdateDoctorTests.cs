using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Doctors;

public class UpdateDoctorTests : IntegrationTestsBase
{
    public UpdateDoctorTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task PUT_UpdateDoctor_NullPassed_ReturnsBadRequest()
    {
        // Arrange
        DoctorUpdateRequest request = null;

        // Act
        var response = await client.PutAsJsonAsync("api/v1/doctors", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PUT_UpdateDoctor_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        DoctorUpdateRequest request = fixture.Build<DoctorUpdateRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .With(x => x.Email, "testEmail@gmail.com")
            .With(x => x.PhoneNumber, "+380959999999")
            .Create();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/doctors", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_UpdateDoctor_ValidObject_ReturnsOK()
    {
        // Arrange
        var doctor = GetTestDoctor();

        await context.Doctors.AddAsync(doctor);
        await context.SaveChangesAsync();

        DoctorUpdateRequest request = fixture.Build<DoctorUpdateRequest>()
            .With(x => x.Id, doctor.Id)
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .With(x => x.Email, "testEmail@gmail.com")
            .With(x => x.PhoneNumber, "+380959999999")
            .Create();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/doctors", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
