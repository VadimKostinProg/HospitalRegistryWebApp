using FluentAssertions;

namespace HospitalRegistry.Tests.IntegrationTests.Patients
{
    public class DeletePatientTests : IntegrationTestsBase
    {
        public DeletePatientTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task DELETE_DeletePatient_IncorrectId_ReturnsNotFound()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"api/v1/patients/{idToPass}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DELETE_DeletePatient_ValidId_ReturnsOK()
        {
            // Arrange
            var patientToDelete = GetTestPatient();

            await context.Patients.AddAsync(patientToDelete);
            await context.SaveChangesAsync();

            var idToPass = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"api/v1/patients/{patientToDelete.Id}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
