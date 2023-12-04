using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public class GetDeletedPatientsListAsyncTests : PatientsServiceTestsBase
{
    [Theory]
    [InlineData(5, 1)]
    [InlineData(4, 2)]
    [InlineData(3, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    public async Task GetDeletedPatientsListAsync_ReturnAllPatients(int pageSize, int pageNumber)
    {
        // Arrange
        var testName = "testName";
        var testSurname = "testSurname";
        var testPatronymic = "testPatronymic";
        var testDateOfBirth = DateOnly.Parse("01.01.2000");

        var patients = GetTestPatients(isDeleted: true).ToList();
        patients[0].Name = patients[3].Name = patients[5].Name = testName;
        patients[0].Surname = patients[3].Surname = patients[5].Surname = testSurname;
        patients[0].Patronymic = patients[3].Patronymic = patients[5].Patronymic = testPatronymic;

        var filteredPatients = patients
            .Where(x => x.Name == testName &&
                        x.Surname == testSurname &&
                        x.Patronymic == testPatronymic)
            .ToList();

        var patientsToReturn = filteredPatients
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        repositoryMock.Setup(x => x.GetAsync<Patient>(It.IsAny<ISpecification<Patient>>(), false))
            .ReturnsAsync(patientsToReturn);
        repositoryMock.Setup(x => x.CountAsync<Patient>(It.IsAny<Expression<Func<Patient, bool>>>()))
            .ReturnsAsync(filteredPatients.Count);

        var exprectedPatients = patientsToReturn
        .Select(x => x.ToPatientResponse())
        .ToList();

        var expectedTotalPages = (int)Math.Ceiling((double)filteredPatients.Count() / pageSize);

        var specifications = new PatientSpecificationsDTO
        {
            Name = testName,
            Surname = testSurname,
            Patronymic = testPatronymic,
            DateOfBirth = testDateOfBirth,
            SortField = "Name",
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        // Act
        var actual = await service.GetDeletedPatientsListAsync(specifications);

        // Assert
        actual.Should().NotBeNull();
        actual.List.Should().BeEquivalentTo(exprectedPatients);
        actual.TotalPages.Should().Be(expectedTotalPages);
    }
}
