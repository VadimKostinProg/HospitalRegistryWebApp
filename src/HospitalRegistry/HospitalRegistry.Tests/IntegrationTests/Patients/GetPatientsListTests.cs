using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Patients;

public class GetPatientsListTests : IntegrationTestsBase
{
    public GetPatientsListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_PatientsList_IncorrectSpecifications_ReturnsBadRequest()
    {
        // Assert
        int pageNumber = -1;
        int pageSize = -1;

        // Act
        var response = await client.GetAsync($"api/v1/patients?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_PatientsList_ValidSpecifications_ReturnsOK()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var response = await client.GetAsync($"api/v1/patients?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
