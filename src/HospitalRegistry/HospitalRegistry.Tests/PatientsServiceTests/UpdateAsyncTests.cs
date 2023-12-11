using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class UpdateAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task UpdateAsync_NullPassed_ThrowsArgumentNullExcpetion()
    {
        // Arrange
        PatientUpdateRequest request = null;

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsKeyNotFoundExcpetion()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(false);

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidName_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.Name, string.Empty)
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidSurname_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.Surname, string.Empty)
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidPatronymic_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.Patronymic, string.Empty)
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_IncorrectDateOfBirth_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.DateOfBirth, DateOnly.FromDateTime(DateTime.Now.AddYears(2)))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>()))
            .ReturnsAsync(true);

        // Assert
        var action = async () =>
        {
            // Act
            await service.UpdateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_SuccessfullUpdating()
    {
        // Arrange
        var request = fixture.Build<PatientUpdateRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Patient, bool>>>()))
            .ReturnsAsync(true);
        repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        // Act
        var response = await service.UpdateAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().Be(request.Name);
        response.Surname.Should().Be(request.Surname);
        response.Patronymic.Should().Be(request.Patronymic);
        response.DateOfBirth.Should().Be(request.DateOfBirth.ToShortDateString());
        response.Email.Should().Be(request.Email);
        response.PhoneNumber.Should().Be(request.PhoneNumber);
    }
}