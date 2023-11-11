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

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetAllAsyncTests : DoctorsServiceTestsBase
    {
        public GetAllAsyncTests() : base() { }

        [Fact]
        public async Task GetAllAsync_ReturnsAllDoctors()
        {
            // Arrange
            var testDoctors = GetTestDoctors().AsQueryable();
            repositoryMock.Setup(x => x.GetFilteredAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>(), true))
                .ReturnsAsync(testDoctors);

            // Act
            var actual = await service.GetAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(testDoctors.Count(), actual.Count());
        }
    }
}
