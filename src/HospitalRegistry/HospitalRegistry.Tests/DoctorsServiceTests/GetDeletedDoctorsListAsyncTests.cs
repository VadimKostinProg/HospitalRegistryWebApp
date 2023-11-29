using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests;

public class GetDeletedDoctorsListAsyncTests : DoctorsServiceTestsBase
{
    [Fact]
    public async Task GetDeletedDoctorsListAsync_ReturnsAllDoctors()
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
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(testDoctors.Count(), actual.Count());
    }
}
