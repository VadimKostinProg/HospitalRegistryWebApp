using System.Linq.Expressions;
using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class UpdateAsyncTests : DiagnosesServiceTestsBase
{
    [Fact]
    public async Task UpdateAsync_NullPassed_ThrowsArgumentNullException()
    {
        // Arrange
        DiagnosisUpdateRequest request = null;
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            var response = await service.UpdateAsync(request);
        });
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var request = fixture.Create<DiagnosisUpdateRequest>();
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(false);
        
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            var response = await service.UpdateAsync(request);
        });
    }
    
    [Fact]
    public async Task UpdateAsync_InvalidName_ThrowsArgumentException()
    {
        // Arrange
        var request = fixture.Build<DiagnosisUpdateRequest>()
            .With(x => x.Name, string.Empty)
            .Create();
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(true);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            var response = await service.UpdateAsync(request);
        });
    }

    [Fact]
    public async Task UpdateAsync_ValidObject_SuccessfullUpdating()
    {
        // Arrange
        var request = fixture.Create<DiagnosisUpdateRequest>();
        repositoryMock.Setup(x => x.ContainsAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(true);
        
        // Act
        var response = await service.UpdateAsync(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(request.Id, response.Id);
        Assert.Equal(request.Name, response.Name);
    }
}