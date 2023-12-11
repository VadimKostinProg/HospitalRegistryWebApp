using AutoFixture;
using FluentAssertions;
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
        DiagnosisCreateRequest request = null;
        
        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.CreateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task CreateAsync_InvalidName_ThrowsArgumentException()
    {
        // Arrange
        var request = new DiagnosisCreateRequest()
        {
            Name = string.Empty
        };

        // Assert
        var action = async () =>
        {
            // Act
            var response = await service.CreateAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task CreateAsync_ValidObject_SuccessfullCreating()
    {
        // Arrange
        var request = fixture.Create<DiagnosisCreateRequest>();
        repositoryMock.Setup(x => x.AddAsync(It.IsAny<Diagnosis>())).Returns(Task.CompletedTask);
        
        // Act
        var response = await service.CreateAsync(request);
        
        // Assert
        response.Should().NotBeNull();
        response.Name.Should().Be(request.Name);
    }
}