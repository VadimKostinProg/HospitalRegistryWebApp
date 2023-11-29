using FluentAssertions;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.RepositoryTests;

public class GetFilteredAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task GetFilteredAsync_FilterPassed_ShouldReturnFilteredList()
    {
        // Arrange
        ISpecification<Doctor> specification = null;

        Expression<Func<Doctor, bool>> predicate = x => x.Name == "test name";

        var expectedDoctors = doctorsList.AsQueryable().Where(predicate).ToList();

        // Act
        var doctors = await repository.GetFilteredAsync<Doctor>(predicate);

        // Assert
        doctors.Should().NotBeNull();
        doctors.Should().BeEquivalentTo(expectedDoctors);
    }
}
