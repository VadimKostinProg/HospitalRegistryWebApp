using AutoFixture;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetAppointmentsHistoryOfPatientAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetAppointmentsHistoryOfPatientAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, false))
            .ReturnsAsync(null as Patient);

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

        var appointments = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(12, 0), new TimeOnly(14, 0));
        patient.Appointments = appointments.ToList();

        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(patient.Id, false))
            .ReturnsAsync(patient);

        // Act
        var history = await service.GetAppointmentsHistoryOfPatientAsync(patient.Id);

        // Assert
        Assert.NotNull(history);
        Assert.True(history.All(x => x.Patient.Id == patient.Id));
    }
}