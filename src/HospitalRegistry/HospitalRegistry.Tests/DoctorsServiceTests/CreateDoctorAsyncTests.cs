using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class CreateDoctorAsyncTests : DoctorsServiceTestsBase
    {
        public CreateDoctorAsyncTests() : base() { }

        [Fact]
        public async Task CreateAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            DoctorAddRequest request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await service.CreateAsync(request);
            });
        }

        [Fact]
        public async Task CreateAsync_InvalidName_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.Name, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.CreateAsync(request);
            });
        }

        [Fact]
        public async Task CreateAsync_InvalidSurname_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.Surname, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.CreateAsync(request);
            });
        }

        [Fact]
        public async Task CreateAsync_InvalidPatronymic_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.Patronymic, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.CreateAsync(request);
            });
        }

        [Fact]
        public async Task CreateAsync_AgeLessThan18_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.DateOfBirth, DateOnly.FromDateTime(DateTime.Now))
                .Create();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.CreateAsync(request);
            });
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_SuccessfullCreation()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.AddAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // Act
            var response = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.Name, response.Name);
            Assert.Equal(request.Surname, response.Surname);
            Assert.Equal(request.Patronymic, response.Patronymic);
            Assert.Equal(request.DateOfBirth.ToShortDateString(), response.DateOfBirth);
            Assert.Equal(request.Email, response.Email);
            Assert.Equal(request.PhoneNumber, response.PhoneNumber);
        }
    }
}
