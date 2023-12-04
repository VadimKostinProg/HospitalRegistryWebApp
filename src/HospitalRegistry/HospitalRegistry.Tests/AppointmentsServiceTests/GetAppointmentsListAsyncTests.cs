using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.AppointmentsServiceTests
{
    public class GetAppointmentsListAsyncTests : AppointmentsServiceTestsBase
    {
        [Fact]
        public async Task GetAppointmentsListAsync_SpecificationsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            AppointmentSpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var list = await service.GetAppointmentsListAsync(specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(4, 2)]
        [InlineData(3, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public async Task GetAppointmentsListAsync_ReturnsAllPatiens(int pageSize, int pageNumber)
        {
            // Arrange
            var doctor = GetTestDoctor();
            var patient = GetTestPatient();
            var diagnoses = GetTestDiagnosis();

            var appointments = GetTestCompletedAppointments(doctor, patient, diagnoses, 
                new TimeOnly(10, 0, 0), 
                new TimeOnly(10, 0, 0))
                .OrderBy(x => x.Id)
                .ToList();

            repositoryMock.Setup(x => x.ContainsAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.ContainsAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.ContainsAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.GetAsync<Appointment>(It.IsAny<ISpecification<Appointment>>(), false))
                .ReturnsAsync(appointments);
            repositoryMock.Setup(x => x.CountAsync<Appointment>(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .ReturnsAsync(appointments.Count);

            var expectedAppointments = appointments
                .Select(appointment => new AppointmentResponse
                {
                    Id = appointment.Id,
                    DateAndTime = appointment.DateAndTime,
                    Doctor = appointment.Doctor.ToDoctorResponse(),
                    Patient = appointment.Patient.ToPatientResponse(),
                    AppointmentType = (AppointmentType)Enum.Parse<AppointmentType>(appointment.AppointmentType),
                    ExtraClinicalData = appointment.ExtraClinicalData,
                    Diagnosis = appointment.Diagnosis?.Name,
                    Status = (AppointmentStatus)Enum.Parse<AppointmentStatus>(appointment.Status),
                    Conclusion = appointment.Conclusion
                })
                .ToList();

            var expectedTotalPages = (int)Math.Ceiling((double)appointments.Count() / pageSize);

            var specifications = new AppointmentSpecificationsDTO
            {
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                DiagnosisId = diagnoses.Id,
                Date = new DateOnly(2023, 10 , 23),
                Type = AppointmentType.Consultation,
                Status = AppointmentStatus.Completed,
                SortField = "Id",
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            // Act
            var actual = await service.GetAppointmentsListAsync(specifications);

            // Assert
            actual.Should().NotBeNull();
            actual.List.Should().BeEquivalentTo(expectedAppointments);
            actual.TotalPages.Should().Be(expectedTotalPages);
        }
    }
}
