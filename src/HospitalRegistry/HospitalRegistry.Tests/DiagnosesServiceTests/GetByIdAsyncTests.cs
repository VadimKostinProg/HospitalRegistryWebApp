using AutoFixture;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetByIdAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Attange
        var idToPass = Guid.NewGuid();
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, true)).ReturnsAsync(null as Diagnosis);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            // Act
            var response = await service.GetByIdAsync(idToPass);
        });
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsObject()
    {
        // Arrange
        var diagnosis = GetTestDiagnosis();
        var idToPass = diagnosis.Id;
        repositoryMock.Setup(x => x.GetByIdAsync<Diagnosis>(idToPass, true))
            .ReturnsAsync(diagnosis);
        
        // Act
        var response = await service.GetByIdAsync(idToPass);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(diagnosis.Id, response.Id);
        Assert.Equal(diagnosis.Name, response.Name);
    }
}