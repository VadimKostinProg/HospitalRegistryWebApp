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
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            await service.DeleteAsync(idToPass);
        });
    }
}