using AutoFixture;
using Azure.Core;
using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetByIdAsyncTests : DoctorsServiceTestsBase
    {
        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
                .ReturnsAsync(null as Doctor);

            // Assert
            var action = async () =>
            {
                // Act
                await service.GetByIdAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsValidObject()
        {
            // Arrange
            var doctor = GetTestDoctor();
            var idToPass = doctor.Id;
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
                .ReturnsAsync(doctor);

            // Act
            var response = await service.GetByIdAsync(idToPass);

            // Assert
            response.Should().NotBeNull();
            response.Name.Should().Be(doctor.Name);
            response.Surname.Should().Be(doctor.Surname);
            response.Patronymic.Should().Be(doctor.Patronymic);
            response.DateOfBirth.Should().Be(doctor.DateOfBirth);
            response.Specialty.ToString().Should().Be(doctor.Specialty);
            response.Email.Should().Be(doctor.Email);
            response.PhoneNumber.Should().Be(doctor.PhoneNumber);
        }
    }
}
