using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetDoctorsListAsyncTests : DoctorsServiceTestsBase
    {
        [Fact]
        public async Task GetDoctorsListAsync_ReturnsAllDoctors()
        {
            // Arrange
            var testDoctors = GetTestDoctors().ToList();
            repositoryMock.Setup(x => x.GetAsync<Doctor>(It.IsAny<ISpecification<Doctor>>(), true))
                .ReturnsAsync(testDoctors);
            var specifications = fixture.Build<DoctorSpecificationsDTO>()
                .With(x => x.DateOfBirth, DateOnly.Parse("01.01.2000"))
                .Create();

            // Act
            var actual = await service.GetDoctorsListAsync(specifications);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().NotBeEmpty();
            actual.Count().Should().Be(testDoctors.Count());
        }
    }
}
