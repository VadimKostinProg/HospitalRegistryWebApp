using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Patients;

public class UpdatePatientTests : IntegrationTestsBase
{
    public UpdatePatientTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task PUT_UpdatePatient_NullPassed_ReturnsBadRequest()
    {
        // Arrange
        PatientUpdateRequest request = null;

        // Act
        var response = await client.PutAsJsonAsync("api/v1/patients", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PUT_UpdatePatient_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        PatientUpdateRequest request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .With(x => x.Email, "testEmail@gmail.com")
            .With(x => x.PhoneNumber, "+380959999999")
            .Create();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/patients", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_UpdatePatient_ValidObject_ReturnsOK()
    {
        // Arrange
        var patient = GetTestPatient();

        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        PatientUpdateRequest request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.Id, patient.Id)
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .With(x => x.Email, "testEmail@gmail.com")
            .With(x => x.PhoneNumber, "+380959999999")
            .Create();

        // Act
        var response = await client.PutAsJsonAsync("api/v1/patients", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
