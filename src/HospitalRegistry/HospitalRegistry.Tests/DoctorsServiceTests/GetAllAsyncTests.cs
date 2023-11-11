using AutoFixture;
using HospitalRegistry.Application.DTO;
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
    public class GetAllAsyncTests : DoctorsServiceTestsBase
    {
        public GetAllAsyncTests() : base() { }

        [Fact]
        public async Task GetAllAsync_ReturnsAllDoctors()
        {
            // Arrange
            var testDoctors = GetTestDoctors().ToList();
            var query = testDoctors.AsQueryable();
            repositoryMock.Setup(x => x.GetFilteredAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), true))
                .ReturnsAsync(query);
            var specifications = new Specifications();
            specificationsServiceMock.Setup(x => x.ApplySpecifications(query, specifications))
                .Returns(query);

            // Act
            var actual = await service.GetAllAsync(specifications);

            // Assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(testDoctors.Count(), actual.Count());
        }
    }
}
