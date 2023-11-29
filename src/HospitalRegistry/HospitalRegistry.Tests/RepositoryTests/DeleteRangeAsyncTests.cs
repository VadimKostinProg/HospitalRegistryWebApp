using FluentAssertions;

namespace HospitalRegistry.Tests.RepositoryTests;

public class DeleteRangeAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task DeleteRangeAsync_ObjectsPassed_DeletesObjectsAndRetunsAmountOfDeletedObjects()
    {
        // Arrange
        int amountToDelete = 5;
        var doctorsToDelete = doctorsList.Take(amountToDelete);

        // Act
        var result = await repository.DeleteRangeAsync(doctorsToDelete);

        var allDoctors = context.Doctors.ToList();

        // Assert
        result.Should().Be(amountToDelete);
        allDoctors.Should().NotContain(doctorsToDelete);
    }
}
