using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Doctors;

public class DeleteDoctorTests : IntegrationTestsBase
{
    public DeleteDoctorTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task DELETE_DeleteDoctor_IncorrectId_ReturnsNotFound()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"api/v1/doctors/{idToPass}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_DeleteDoctor_ValidId_ReturnsOK()
    {
        // Arrange
        var doctorToDelete = GetTestDoctor();

        await context.Doctors.AddAsync(doctorToDelete);
        await context.SaveChangesAsync();

        var idToPass = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"api/v1/doctors/{doctorToDelete.Id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
