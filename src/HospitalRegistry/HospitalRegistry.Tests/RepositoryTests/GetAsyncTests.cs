using FluentAssertions;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.RepositoryTests;

public class GetAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task GetAsync_SpecificationNull_ReturnsAllEntities()
    {
        // Arrange
        ISpecification<Doctor> specification = null;

        // Act
        var doctors = await repository.GetAsync(specification);

        // Assert
        doctors.Should().NotBeNull();
        doctors.Should().BeEquivalentTo(this.doctorsList);
    }

    [Theory]
    [InlineData(5, 2)]
    [InlineData(2, 3)]
    [InlineData(4, 1)]
    [InlineData(1, 5)]
    [InlineData(3, 6)]
    public async Task GetAsync_PaginationPassed_ReturnsPagedEntitiesList(int pageSize, int pageNumber)
    {
        // Arrange
        var specification = new SpecificationBuilder<Doctor>()
            .WithPagination(pageSize, pageNumber)
            .Build();

        int take = pageSize;
        int skip = (pageNumber - 1) * pageSize;

        var expectedDoctors = this.doctorsList
            .Skip(skip)
            .Take(take)
            .ToList();

        // Act
        var doctors = await repository.GetAsync(specification);

        // Assert
        doctors.Should().NotBeNull();
        doctors.Should().BeEquivalentTo(expectedDoctors);
    }

    [Fact]
    public async Task GetAsynk_SortingPassed_ReturnsSortedEntitiesList()
    {
        // Arrange
        var specification = new SpecificationBuilder<Doctor>()
            .OrderBy(x => x.Name, SortDirection.ASC)
            .Build();

        var expectedDoctors = this.doctorsList
            .OrderBy(x => x.Name)
            .ToList();

        // Act
        var doctors = await repository.GetAsync(specification);

        // Assert
        doctors.Should().NotBeNull();
        doctors.Should().BeEquivalentTo(expectedDoctors);
    }

    [Fact]
    public async Task GetAsynk_FilteringPassed_ReturnsSortedEntitiesList()
    {
        // Arrange
        var specification = new SpecificationBuilder<Doctor>()
            .With(x => x.Name == "test name")
            .Build();

        var expectedDoctors = this.doctorsList
            .Where(x => x.Name == "test name")
            .ToList();

        // Act
        var doctors = await repository.GetAsync(specification);

        // Assert
        doctors.Should().NotBeNull();
        doctors.Should().BeEquivalentTo(expectedDoctors);
    }
}
