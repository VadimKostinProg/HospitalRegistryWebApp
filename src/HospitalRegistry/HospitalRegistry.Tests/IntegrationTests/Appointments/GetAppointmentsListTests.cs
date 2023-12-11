using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Appointments;

public class GetAppointmentsListTests : IntegrationTestsBase
{
    public GetAppointmentsListTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GET_AppointmentsList_IncorrectSpecifications_ReturnsBadRequest()
    {
        // Assert
        int pageNumber = -1;
        int pageSize = -1;

        // Act
        var response = await client.GetAsync($"api/v1/appointments?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_AppointmentsList_ValidSpecifications_ReturnsOK()
    {
        // Assert
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var response = await client.GetAsync($"api/v1/appointments?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
