using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public class GetDiagnosesListAsyncTests : DiagnosesServiceTestsBase
{
    [Theory]
    [InlineData(5, 1)]
    [InlineData(4, 2)]
    [InlineData(3, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    public async Task GetDiagnosesListAsync_ReturnsAllDiagnoses(int pageSize, int pageNumber)
    {
        // Arrange
        var testName = "testName";
        var diagnoses = GetTestDiagnoses().ToList();
        diagnoses[0].Name = diagnoses[3].Name = diagnoses[5].Name = testName;

        var filteredDiagnoses = diagnoses
            .Where(x => x.Name == testName)
            .ToList();

        var diagnosesToReturn = filteredDiagnoses
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        repositoryMock.Setup(x => x.GetAsync<Diagnosis>(It.IsAny<ISpecification<Diagnosis>>(), true))
            .ReturnsAsync(diagnosesToReturn);
        repositoryMock.Setup(x => x.CountAsync<Diagnosis>(It.IsAny<Expression<Func<Diagnosis, bool>>>()))
            .ReturnsAsync(filteredDiagnoses.Count);

        var exprectedDiagnoses = diagnosesToReturn
            .Select(x => x.ToDiagnosisResponse())
            .ToList();

        var expectedTotalPages = (int)Math.Ceiling((double)filteredDiagnoses.Count() / pageSize);

        var specifications = new DiagnosisSpecificationsDTO
        {
            Name = testName,
            SortField = "Name",
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        // Act
        var actual = await service.GetDiagnosesListAsync(specifications);

        // Assert
        actual.Should().NotBeNull();
        actual.List.Should().BeEquivalentTo(exprectedDiagnoses);
        actual.TotalPages.Should().Be(expectedTotalPages);
    }
}