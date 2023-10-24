using AutoFixture;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetByIdAsyncTests : DoctorsServiceTestsBase
    {
        public GetByIdAsyncTests() : base() { }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();
            repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass)).ReturnsAsync(null as Doctor);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                // Act
                var response = await service.GetByIdAsync(idToPass);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsValidObject()
        {
            // Arrange
            var doctor = fixture.Create<Doctor>();
            var idToPass = doctor.Id;
            repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass)).ReturnsAsync(doctor);

            // Act
            var response = await service.GetByIdAsync(idToPass);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(doctor.Id, response.Id);
            Assert.Equal(doctor.Surname, response.Surname);
            Assert.Equal(doctor.Patronymic, response.Patronymic);
            Assert.Equal(doctor.DateOfBirth, response.DateOfBirth);
            Assert.Equal(doctor.Email, response.Email);
            Assert.Equal("+" + doctor.PhoneNumber.ToString(), response.PhoneNumber);
        }
    }
}
