using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class GetAppointmentsHistoryOfDoctorAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task GetAppointmentsHistoryOfDoctorAsync_SpecificationIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        SpecificationsDTO specifications = null;

        // Assert
        var action = async () =>
        {
            // Act
            var history = await service.GetAppointmentsHistoryOfDoctorAsync(idToPass, specifications);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetAppointmentsHistoryOfDoctorAsync_IncorrectId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        repositoryMock.Setup(x => x.ContainsAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()))
            .ReturnsAsync(false);

        var specifications = fixture.Create<SpecificationsDTO>();

        // Assert
        var action = async () =>
        {
            // Act
            var history = await service.GetAppointmentsHistoryOfDoctorAsync(idToPass, specifications);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [InlineData(5, 1)]
    [InlineData(4, 2)]
    [InlineData(3, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    public async Task GetAppointmentsHistoryOfDoctorAsync_ValidId_ReturnsAppointmentsHistory(int pageSize, int pageNumber)
    {
        // Arrange
        var patient = GetTestPatient();
        var doctor = GetTestDoctor();
        var diagnosis = new Diagnosis
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
        };

        var appointments = GetTestCompletedAppointments(doctor, patient, diagnosis, new TimeOnly(9, 0), new TimeOnly(17, 0));

        var filteredAppointments = appointments
            .OrderByDescending(x => x.DateAndTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        repositoryMock.Setup(x => x.ContainsAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()))
            .ReturnsAsync(true);
        repositoryMock.Setup(x => x.GetAsync<Appointment>(It.IsAny<ISpecification<Appointment>>(), false))
            .ReturnsAsync(filteredAppointments);
        repositoryMock.Setup(x => x.CountAsync<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>()))
            .ReturnsAsync(appointments.Count());

        var expectedAppointments = filteredAppointments
            .Select(appointment => new AppointmentResponse()
            {
                Id = appointment.Id,
                DateAndTime = appointment.DateAndTime,
                Doctor = appointment.Doctor.ToDoctorResponse(),
                Patient = patient.ToPatientResponse(),
                Diagnosis = diagnosis.Name,
                AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
                ExtraClinicalData = appointment.ExtraClinicalData,
                Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
                Conclusion = appointment.Conclusion
            })
            .ToList();

        var expectedTotalPages = (int)Math.Ceiling((double)appointments.Count() / pageSize);

        var specifications = new SpecificationsDTO
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        // Act
        var history = await service.GetAppointmentsHistoryOfDoctorAsync(doctor.Id, specifications);

        // Assert
        history.Should().NotBeNull();
        history.List.Should().BeEquivalentTo(expectedAppointments);
        history.TotalPages.Should().Be(expectedTotalPages);
    }
}