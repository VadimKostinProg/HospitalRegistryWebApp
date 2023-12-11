using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using System.Net.Http.Json;

namespace HospitalRegistry.Tests.IntegrationTests.Patients
{
    public class CreatePatientTests : IntegrationTestsBase
    {
        public CreatePatientTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task POST_CreatePatient_NullPassed_ReturnsBadRequest()
        {
            // Arrange
            PatientAddRequest request = null;

            // Act
            var response = await client.PostAsJsonAsync("api/v1/patients", request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CreatePatient_ValidObject_ReturnsOK()
        {
            // Arrange
            PatientAddRequest request = fixture.Build<PatientAddRequest>()
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .With(x => x.Email, "testEmail@gmail.com")
                .With(x => x.PhoneNumber, "+380959999999")
                .Create();

            // Act
            var response = await client.PostAsJsonAsync("api/v1/patients", request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
