using FluentAssertions;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class DeleteDiagnosisTests : IntegrationTestsBase
{
    public DeleteDiagnosisTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task DELETE_DeleteDiagnosis_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"api/v1/diagnoses/{idToPass}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_DeleteDiagnosis_ValidId_ReturnsOK()
    {
        // Arrange
        var diagnosesToDelete = GetTestDiagnosis();

        context.Diagnoses.Add(diagnosesToDelete);
        context.SaveChanges();

        // Act
        var response = await client.DeleteAsync($"api/v1/diagnoses/{diagnosesToDelete.Id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
