using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class CreateDiagnosisTests : IntegrationTestsBase
{
    public CreateDiagnosisTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task POST_CreateDiagnosis_UserUnauthorized_ReturnsUnauthorized()
    {
        // Arrange
        client.DefaultRequestHeaders.Authorization = null;

        DiagnosisCreateRequest request = null;

        // Act
        var response = await client.PostAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_CreateDiagnosis_NullPassedToBody_ReturnsBadRequest()
    {
        // Arrange
        DiagnosisCreateRequest request = null;

        // Act
        var response = await client.PostAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_CreateDiagnosis_NameAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var name = "test diagnosis";

        context.Diagnoses.Add(new Diagnosis { Name = name });
        context.SaveChanges();

        DiagnosisCreateRequest request = new()
        {
            Name = name
        };

        // Act
        var response = await client.PostAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_CreateDiagnosis_ValidObject_ReturnsOK()
    {
        // Arrange
        DiagnosisCreateRequest request = new()
        {
            Name = "new diagnosis"
        };

        // Act
        var response = await client.PostAsJsonAsync("api/v1/diagnoses", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
