using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Diagnoses;

public class GetDeletedDiagnosesListTests : IntegrationTestsBase
{
    public GetDeletedDiagnosesListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_DeletedDiagnosisList_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await client.GetAsync($"api/v1/diagnoses/deleted");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_DeletedDiagnosisList_InvalidSpecifications_ReturnsBadRequest()
    {
        // Arrange
        var pageSize = -1;
        var pageNumber = -1;

        // Act
        var response = await client.GetAsync($"api/v1/diagnoses/deleted?pageSize={pageSize}&pageNumber={pageNumber}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_DeletedDiagnosisList_ValidSpecifications_ReturnsOk()
    {
        // Arrange
        var pageSize = 1;
        var pageNumber = 1;

        // Act
        var response = await client.GetAsync($"api/v1/diagnoses/deleted?pageSize={pageSize}&pageNumber={pageNumber}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
