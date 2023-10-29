using AutoFixture;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetAppointmentsHistoryOfDoctorAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetAppointmentsHistoryOfDoctorAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass))
            .ReturnsAsync(null as Doctor);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            var history = await service.GetAppointmentsHistoryOfPatientAsync(idToPass);
        });
    }

    [Fact]
    public async Task GetAppointmentsHistoryOfPatientAsync_ValidId_ReturnsAppointmentsHistory()
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var diagnosis = new Diagnosis 
        { 
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
        };
        doctor.Schedules = GetTestSchedules(doctor.Id).ToList();

        var appointments = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0));
        doctor.Appointments = appointments.ToList();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(doctor.Id))
            .ReturnsAsync(doctor);

        // Act
        var history = await service.GetAppointmentsHistoryOfDoctorAsync(doctor.Id);

        // Assert
        Assert.NotNull(history);
        Assert.True(history.All(x => x.Doctor.Id == doctor.Id));
    }
}