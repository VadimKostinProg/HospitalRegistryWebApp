using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class DeleteAsyncTests : PatientsServiceTestsBase
{
    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Patient>(idToPass, true))
            .ReturnsAsync(null as Patient);
        
        // Assert
        var action = async () =>
        {
            // Act
            await service.DeleteAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
}