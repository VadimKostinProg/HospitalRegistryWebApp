using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var testDoctors = GetTestDoctors().ToList();
            repositoryMock.Setup(x => x.GetAllAsync<Doctor>(true)).ReturnsAsync(testDoctors);

            // Act
            var actual = await service.GetAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(testDoctors.Count, actual.Count());
        }
    }
}
