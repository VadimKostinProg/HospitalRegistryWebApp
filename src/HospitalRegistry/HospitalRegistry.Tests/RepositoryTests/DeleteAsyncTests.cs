using FluentAssertions;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.RepositoryTests;

public class DeleteAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task DeleteAsync_NotExistanObjectIdPassed_ReturnsFalse()
    {
        // Arrange
        var idToPass = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync<Doctor>(idToPass);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistanObjectIdPassed_ReturnsTrueAndDeletesObject()
    {
        // Arrange
        var doctorToDelete = doctorsList.First();
        var idToPass = doctorToDelete.Id;

        // Act
        var result = await repository.DeleteAsync<Doctor>(idToPass);

        var deletedDoctor = context.Doctors.FirstOrDefault(x => x.Id == idToPass);

        // Assert
        result.Should().BeTrue();
        deletedDoctor.Should().BeNull();
    }
}
