using AutoFixture;
using FluentAssertions;
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
        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, true))
            .ReturnsAsync(null as Doctor);

        // Assert
        var action = async () =>
        {
            // Act
            var history = await service.GetAppointmentsHistoryOfPatientAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
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

        var appointments = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0));
        doctor.Appointments = appointments.ToList();

        repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(doctor.Id, true))
            .ReturnsAsync(doctor);

        // Act
        var history = await service.GetAppointmentsHistoryOfDoctorAsync(doctor.Id);

        // Assert
        history.Should().NotBeNull();
        history.Should().NotBeEmpty();
        history.Should().OnlyContain(x => x.Doctor.Id == doctor.Id);
    }
}