using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.RepositoryTests;

public class ContainsAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task ContainsAsync_IncorrectExpression_ReturnsNull()
    {
        // Arrange
        Expression<Func<Doctor, bool>> expression = x => false;

        // Act
        var result = await repository.ContainsAsync<Doctor>(expression);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ContainsAsync_ValidExpression_ReturnsNull()
    {
        // Arrange
        var expectedDoctor = doctorsList.First();
        Expression<Func<Doctor, bool>> expression = x =>
            x.Name == expectedDoctor.Name &&
            x.Surname == expectedDoctor.Surname;

        // Act
        var result = await repository.ContainsAsync<Doctor>(expression);

        // Assert
        result.Should().BeTrue();
    }
}
