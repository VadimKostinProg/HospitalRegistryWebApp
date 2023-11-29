using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.RepositoryTests;

public class FirstOrDefaultAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task FirstOrDefaultAsync_IncorrectExpression_ReturnsNull()
    {
        // Arrange
        Expression<Func<Doctor, bool>> expression = x => false;

        // Act
        var doctor = await repository.FirstOrDefaultAsync<Doctor>(expression);

        // Assert
        doctor.Should().BeNull();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ValidExpression_ReturnsNull()
    {
        // Arrange
        var expectedDoctor = doctorsList.First();
        Expression<Func<Doctor, bool>> expression = x => 
            x.Name == expectedDoctor.Name && 
            x.Surname == expectedDoctor.Surname;

        // Act
        var doctor = await repository.FirstOrDefaultAsync<Doctor>(expression);

        // Assert
        doctor.Should().NotBeNull();
        doctor.Should().BeEquivalentTo(expectedDoctor);
    }
}
