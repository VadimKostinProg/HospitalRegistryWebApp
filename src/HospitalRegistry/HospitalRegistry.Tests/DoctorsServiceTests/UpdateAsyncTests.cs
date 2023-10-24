using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class UpdateAsyncTests : DoctorsServiceTestsBase
    {
        public UpdateAsyncTests() : base() { }

        [Fact]
        public async Task UpdateAsync_NullPassed_ThrowsArgumentNullExcpetion()
        {
            // Arrange
            DoctorUpdateRequest request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_InvalidId_ThrowsKeyNotFoundExcpetion()
        {
            // Arrange
            DoctorUpdateRequest request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(false);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_InvalidName_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.Name, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_InvalidSurname_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.Surname, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_InvalidPatronymic_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.Patronymic, string.Empty)
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_AgeLessThan18_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.DateOfBirth, DateOnly.FromDateTime(DateTime.Now))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await service.UpdateAsync(request);
            });
        }

        [Fact]
        public async Task UpdateAsync_ValidRequest_SuccessfullUpdating()
        {
            // Arrange
            var request = fixture.Build<DoctorUpdateRequest>()
                .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
                .Create();

            repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(true);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // Act
            var response = await service.UpdateAsync(request);

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
