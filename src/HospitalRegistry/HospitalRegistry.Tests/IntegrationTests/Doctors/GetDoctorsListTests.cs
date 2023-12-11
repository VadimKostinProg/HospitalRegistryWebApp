using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Doctors;

public class GetDoctorsListTests : IntegrationTestsBase
{
    public GetDoctorsListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_DoctorsList_IncorrectSpecifications_ReturnsBadRequest()
    {
        // Assert
        int pageNumber = -1;
        int pageSize = -1;

        // Act
        var response = await client.GetAsync($"api/v1/doctors?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_DoctorsList_ValidSpecifications_ReturnsOK()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var response = await client.GetAsync($"api/v1/doctors?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
