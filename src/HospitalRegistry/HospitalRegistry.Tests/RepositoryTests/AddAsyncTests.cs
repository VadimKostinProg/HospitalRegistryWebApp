using FluentAssertions;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Tests.RepositoryTests;

public class AddAsyncTests : RepositoryTestsBase
{
    [Fact]
    public async Task AddAsync_AddNewEntity_EntityIsPresendInTheDataSource()
    {
        // Arrange
        var entityToInsert = GetTestDoctor();

        // Act
        await repository.AddAsync(entityToInsert);

        var insertedEntity = context.Doctors.FirstOrDefault(x => x.Id == entityToInsert.Id);

        // Assert
        insertedEntity.Should().NotBeNull();
        insertedEntity.Should().BeEquivalentTo(entityToInsert);
    }
}
