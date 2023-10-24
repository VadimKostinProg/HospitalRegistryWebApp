using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class CreateAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task CreateAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        DiagnosisAddRequest request = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.CreateAsync(request);
        });
    }
    
    [Fact]
    public async Task CreateAsync_InvalidName_ThrowsArgumentException()
    {
        // Arrange
        var request = new DiagnosisAddRequest()
        {
            Name = string.Empty
        };
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            var response = await service.CreateAsync(request);
        });
    }
    
    [Fact]
    public async Task CreateAsync_ValidObject_SuccessfullCreating()
    {
        // Arrange
        var request = fixture.Create<DiagnosisAddRequest>();
        repositoryMock.Setup(x => x.AddAsync(It.IsAny<Diagnosis>())).Returns(Task.CompletedTask);
        
        // Act
        var response = await service.CreateAsync(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(request.Name, response.Name);
    }
}