using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetDoctorsListAsyncTests : DoctorsServiceTestsBase
    {
        [Fact]
        public async Task GetDoctorsListAsync_SpecificationsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            DoctorSpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var list = await service.GetDoctorsListAsync(specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(4, 2)]
        [InlineData(3, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public async Task GetDoctorsListAsync_ReturnsAllDoctors(int pageSize, int pageNumber)
        {
            // Arrange
            var testName = "testName";
            var testSurname = "testSurname";
            var testPatronymic = "testPatronymic";
            var testDateOfBirth = DateOnly.Parse("01.01.2000");
            var testSpecialty = Specialty.Allergist;

            var doctors = GetTestDoctors().ToList();
            doctors[0].Name = doctors[3].Name = doctors[5].Name = testName;
            doctors[0].Surname = doctors[3].Surname = doctors[5].Surname = testSurname;
            doctors[0].Patronymic = doctors[3].Patronymic = doctors[5].Patronymic = testPatronymic;

            var filteredDoctors = doctors
                .Where(x => x.Name == testName &&
                            x.Surname == testSurname &&
                            x.Patronymic == testPatronymic)
                .ToList();

            var doctorsToReturn = filteredDoctors
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            repositoryMock.Setup(x => x.GetAsync<Doctor>(It.IsAny<ISpecification<Doctor>>(), false))
                .ReturnsAsync(doctorsToReturn);
            repositoryMock.Setup(x => x.CountAsync<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>()))
                .ReturnsAsync(filteredDoctors.Count);

            var exprectedDoctors = doctorsToReturn
                .Select(x => x.ToDoctorResponse())
                .ToList();

            var expectedTotalPages = (int)Math.Ceiling((double)filteredDoctors.Count() / pageSize);

            var specifications = new DoctorSpecificationsDTO
            {
                Name = testName,
                Surname = testSurname,
                Patronymic = testPatronymic,
                DateOfBirth = testDateOfBirth,
                Specialty = testSpecialty,
                SortField = "Name",
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            // Act
            var actual = await service.GetDoctorsListAsync(specifications);

            // Assert
            actual.Should().NotBeNull();
            actual.List.Should().BeEquivalentTo(exprectedDoctors);
            actual.TotalPages.Should().Be(expectedTotalPages);
        }
    }
}