using System.Linq.Expressions;
using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetScheduledAppoitnmentsOfDoctorAsync : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_InvalidDoctorId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, false))
            .ReturnsAsync(null as Doctor);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_DoctorIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        doctor.IsDeleted = true;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, false))
            .ReturnsAsync(doctor);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_DateLassThanToday_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, false))
            .ReturnsAsync(doctor);

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_ValidParams_RetunsScheduledAppointments()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
        var patient = GetTestPatient();
        var appointments = GetTestScheduledAppointments(doctor, patient, date, new TimeOnly(12, 0)).AsQueryable();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, false))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), false))
            .ReturnsAsync(appointments);

        // Act
        var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().NotBeEmpty();
        response.Should().OnlyContain(x => x.Doctor.Id == doctor.Id &&
                                      DateOnly.FromDateTime(x.DateAndTime) == date);
    }
}