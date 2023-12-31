﻿using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetBySpecialtyAsyncTests : DoctorsServiceTestsBase
    {
        public GetBySpecialtyAsyncTests() : base() { }

        [Theory]
        [InlineData(Specialty.Allergist)]
        [InlineData(Specialty.Urologist)]
        [InlineData(Specialty.Cardiologist)]
        [InlineData(Specialty.Surgeon)]
        [InlineData(Specialty.Radiologist)]
        public async Task GetBySpecialtyAsync_SpecialtyPassed_ReturnsFilteredDoctors(Specialty specialty)
        {
            // Arrange
            var doctors = GetTestDoctors(50).AsQueryable();
            var filteredDoctors = doctors.Where(doctor => doctor.Specialty == specialty.ToString()).AsQueryable();
            repositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), false))
                .ReturnsAsync(filteredDoctors);

            // Act
            var actual = await service.GetBySpecialtyAsync(specialty);

            // Assert
            actual.Should().NotBeNull();
            actual.Count().Should().Be(filteredDoctors.Count());
        }
    }
}
