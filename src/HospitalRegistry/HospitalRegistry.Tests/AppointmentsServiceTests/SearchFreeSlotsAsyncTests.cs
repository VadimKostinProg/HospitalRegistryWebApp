using System.Linq.Expressions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using It = Moq.It;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public class SearchFreeSlotsAsyncTests : AppointmentsServiceTestsBase
{
    [Fact]
    public async Task SearchFreeSlotsAsync_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_StartDateLessTheCurrentDate_ThrowsArgumentException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_StartDateMoreThanEndDate_ThrowsArgumentException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }
    
    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdAndSpecialtyAreBothNull_ThrowsArgumentNullException()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            AppointmentType = AppointmentType.Consultation
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.SearchFreeSlotsAsync(specifications);
        });
    }

    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdIsNotPassed_ReturnsFreeSlotsBySpecialty()
    {
        // Arrange
        FreeSlotsSearchSpecifications specifications = new()
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            AppointmentType = AppointmentType.Consultation,
            Specialty = Specialty.Allergist
        };

        // TODO: setup repositoryMock
        
        // Act
        var response = await service.SearchFreeSlotsAsync(specifications);
        
        // Assert
        // TODO
    }
    
    [Fact]
    public async Task SearchFreeSlotsAsync_DoctorIdIsPassed_ReturnsFreeSlotsByDoctor()
    {
        // TODO
    }
}