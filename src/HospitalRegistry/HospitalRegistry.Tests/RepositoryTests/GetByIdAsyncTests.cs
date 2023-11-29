using FluentAssertions;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.RepositoryTests;

public class GetByIdAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task GetByIdAsync_IncorrectId_ReturnsNull()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        // Act
        var doctor = await repository.GetByIdAsync<Doctor>(idToPass);

        // Assert
        doctor.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsNull()
    {
        // Arrange
        var expectedDoctor = doctorsList.First();
        var idToPass = expectedDoctor.Id;

        // Act
        var doctor = await repository.GetByIdAsync<Doctor>(idToPass);

        // Assert
        doctor.Should().NotBeNull();
        doctor.Should().BeEquivalentTo(expectedDoctor);
    }
}
