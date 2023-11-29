using FluentAssertions;

namespace HospitalRegistry.Tests.RepositoryTests;

public class UpdateAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task UpdateAsync_ExistantObjectPassed_ShouldUpdateObject()
    {
        // Arrange
        var existantDoctor = doctorsList.First();

        string newName = $"{existantDoctor.Name}-new";
        existantDoctor.Name = newName;
        
        // Act
        await repository.UpdateAsync(existantDoctor);

        var updatedDoctor = context.Doctors.FirstOrDefault(x => x.Id == existantDoctor.Id);

        // Assert
        updatedDoctor.Should().NotBeNull();
        updatedDoctor.Should().BeEquivalentTo(existantDoctor);
    }
}
