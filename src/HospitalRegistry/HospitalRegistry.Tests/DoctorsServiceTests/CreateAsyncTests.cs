using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class CreateAsyncTests : DoctorsServiceTestsBase
    {
        public CreateAsyncTests() : base() { }

        [Fact]
        public async Task CreateAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            DoctorAddRequest request = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
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
            var action = async () =>
            {
                // Act
                await service.CreateAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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
            var action = async () =>
            {
                // Act
                await service.CreateAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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
            var action = async () =>
            {
                // Act
                await service.CreateAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_AgeLessThan18_ThrowsArgumentException()
        {
            // Arrange
            var request = fixture.Build<DoctorAddRequest>()
                .With(x => x.DateOfBirth, DateOnly.FromDateTime(DateTime.Now))
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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
            response.Should().NotBeNull();
            response.Name.Should().Be(request.Name);
            response.Surname.Should().Be(request.Surname);
            response.Patronymic.Should().Be(request.Patronymic);
            response.DateOfBirth.Should().Be(request.DateOfBirth.ToShortDateString());
            response.Specialty.Should().Be(request.Specialty);
            response.Email.Should().Be(request.Email);
            response.PhoneNumber.Should().Be(request.PhoneNumber);
        }
    }
}
