using FluentAssertions;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class DeleteAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, true))
            .ReturnsAsync(null as Diagnosis);

        // Assert
        var action = async () =>
        {
            // Act
            await service.DeleteAsync(idToPass);
        };

        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
}