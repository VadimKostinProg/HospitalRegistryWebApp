using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.RepositoryTests
{
    public class CountAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task CountAsync_NoParameters_ReturnsAmountOfAllEntities()
        {
            // Arrange
            var expectedCount = doctorsList.Count;

            // Act
            var result = await repository.CountAsync<Doctor>();

            // Assert
            result.Should().Be(expectedCount);
        }

        [Fact]
        public async Task CountAsync_PredicatePassed_ReturnsAmountOfFilteredEntities()
        {
            // Arrange
            Expression<Func<Doctor, bool>> predicate = x => x.Name == "test name";

            var expectedCount = doctorsList
                .AsQueryable()
                .Where(predicate)
                .Count();

            // Act
            var result = await repository.CountAsync<Doctor>(predicate);

            // Assert
            result.Should().Be(expectedCount);
        }
    }
}
