using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class UpdateDiagnosisTests : IntegrationTestsBase
{
    public UpdateDiagnosisTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task PUT_UpdateDiagnosis_UserUnauthorized_ReturnsUnauthorized()
    {
        // Arrange
        client.DefaultRequestHeaders.Authorization = null;

        DiagnosisUpdateRequest request = null;

        // Act
        var response = await client.PutAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PUT_UpdateDiagnosis_NullPassedToBody_ReturnsBadRequest()
    {
        // Arrange
        DiagnosisUpdateRequest request = null;

        // Act
        var response = await client.PutAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PUT_UpdateDiagnosis_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        DiagnosisUpdateRequest request = new()
        {
            Id = Guid.NewGuid(),
            Name = "updated diagnosis"
        };

        // Act
        var response = await client.PutAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_UpdateDiagnosis_ValidObject_ReturnsOK()
    {
        // Arrange
        var existantDiagnosis = GetTestDiagnosis();

        await context.Diagnoses.AddAsync(existantDiagnosis);
        await context.SaveChangesAsync();

        DiagnosisUpdateRequest request = new()
        {
            Id = existantDiagnosis.Id,
            Name = "updated diagnosis"
        };

        // Act
        var response = await client.PutAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
