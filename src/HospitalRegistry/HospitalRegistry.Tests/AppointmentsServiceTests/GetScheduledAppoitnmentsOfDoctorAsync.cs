using System.Linq.Expressions;
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

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass))
            .ReturnsAsync(null as Doctor);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        });
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_DoctorIsDeleted_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        doctor.IsDeleted = true;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass))
            .ReturnsAsync(doctor);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        });
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_DateLassThanToday_ThrowsArgumentException()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass))
            .ReturnsAsync(doctor);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        });
    }

    [Fact]
    public async Task GetScheduledAppoitnmentsOfDoctor_ValidParams_RetunsScheduledAppointments()
    {
        // Arrange
        var doctor = GetTestDoctor();
        var idToPass = doctor.Id;
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
        var patient = GetTestPatient();
        var appointments = GetTestScheduledAppointments(doctor, patient, date, new TimeOnly(12, 0));

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass))
            .ReturnsAsync(doctor);
        repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), true))
            .ReturnsAsync(appointments);

        // Act
        var response = await service.GetScheduledAppoitnmentsOfDoctorAsync(idToPass, date);
        
        // Assert
        Assert.NotEmpty(response);
        Assert.True(response.All(x => x.Doctor.Id == doctor.Id && 
                                      DateOnly.FromDateTime(x.DateAndTime) == date));
    }
}