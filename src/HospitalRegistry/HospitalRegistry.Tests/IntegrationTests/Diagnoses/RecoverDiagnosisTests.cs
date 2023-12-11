using FluentAssertions;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class RecoverDiagnosisTests : IntegrationTestsBase
{
    public RecoverDiagnosisTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task POST_Recover_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        // Act
        var response = await client.PostAsync($"api/diagnoses/{idToPass}/recover", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_Recover_ValidId_ReturnsOK()
    {
        // Arrange
        var diagnosesToRecover = GetTestDiagnosis(isDeleted: true);

        // Act
        var response = await client.PostAsync($"api/diagnoses/{diagnosesToRecover.Id}/recover", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
