using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class GetDiagnosesListTests : IntegrationTestsBase
{
    public GetDiagnosesListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_DiagnosisList_InvalidSpecifications_ReturnsBadRequest()
    {
        // Arrange
        var pageSize = -1;
        var pageNumber = -1;

        // Act
        var response = await client.GetAsync($"api/v1/diagnoses?pageSize={pageSize}&pageNumber={pageNumber}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_DiagnosisList_ValidSpecifications_ReturnsOk()
    {
        // Arrange
        var pageSize = 1;
        var pageNumber = 1;

        // Act
        var response = await client.GetAsync($"api/v1/diagnoses?pageSize={pageSize}&pageNumber={pageNumber}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
