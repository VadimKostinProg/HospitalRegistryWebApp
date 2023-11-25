using AutoFixture;
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
            DoctorSpecificationsDTO? specifications = null;

            // Act
            var actual = await service.GetDoctorsListAsync(specifications);

            // Assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(testDoctors.Count(), actual.Count());
        }
    }
}
