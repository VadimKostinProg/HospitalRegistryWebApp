using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class CreateAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task CreateAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        PatientAddRequest request = null;

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
        var request = fixture.Build<PatientAddRequest>()
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
        var request = fixture.Build<PatientAddRequest>()
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
        var request = fixture.Build<PatientAddRequest>()
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
    public async Task CreateAsync_IncorrectDateOfBirth_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<PatientAddRequest>()
            .With(x => x.DateOfBirth, DateOnly.FromDateTime(DateTime.Now.AddYears(2)))
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
        var request = fixture.Build<PatientAddRequest>()
            .With(x => x.DateOfBirth, new DateOnly(2000, 1, 1))
            .Create();

        repositoryMock.Setup(x => x.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        // Act
        var response = await service.CreateAsync(request);

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