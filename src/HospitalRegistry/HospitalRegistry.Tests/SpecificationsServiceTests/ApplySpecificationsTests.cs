using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Tests.SpecificationsServiceTests
{
    public class ApplySpecificationsTests : SpecificationsServiceTestsBase
    {
        [Fact]
        public void ApplySpecifications_EnterFilteringSpecifications_ReturnFilteredItems()
        {
            // Arrange
            var doctors = GetTestDoctors(count: 20).ToList();
            doctors[0].Specialty = doctors[1].Specialty = doctors[2].Specialty = Specialty.Psychiatrist.ToString();

            var query = doctors.AsQueryable();

            var specifications = new Specifications
            {
                Filters = new Dictionary<string, string>()
                {
                    { "Specialty", Specialty.Psychiatrist.ToString() }
                }
            };

            // Act
            var result = service.ApplySpecifications(query, specifications)?.ToList();

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(result.All(x => x.Specialty == Specialty.Psychiatrist.ToString()));
        }

        [Fact]
        public void ApplySpecifications_EnterSortingSpecifications_ReturnSortedItems()
        {
            // Arrange
            var doctors = GetTestDoctors().ToList();
            var query = doctors.AsQueryable();
            var expectedSortedDoctors = doctors.OrderBy(x => x.Name).ToList();

            var specifications = new Specifications
            {
                SortField = "Name",
                SortDirection = SortDirection.ASC,
            };

            // Act
            var actualSortedDoctors = service.ApplySpecifications(query, specifications)?.ToList();

            // Assert
            Assert.NotNull(actualSortedDoctors);
            Assert.True(actualSortedDoctors.Any());
            Assert.True(expectedSortedDoctors.SequenceEqual(actualSortedDoctors));
        }

        [Fact]
        public void ApplySpecifications_EnterSearchingSpecifications_ReturnSearchingResult()
        {
            // Arrange
            var doctors = GetTestDoctors().ToList();
            var query = doctors.AsQueryable();
            var searchTerm = doctors.First().Name;

            var specifications = new Specifications
            {
                SearchTerm = searchTerm
            };

            // Act
            var result = service.ApplySpecifications(query, specifications)?.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(doctors.First(), result);
        }

        [Fact]
        public void ApplySpecifications_EnterPaginationParams_ReturnSearchingResult()
        {
            // Arrange
            var doctors = GetTestDoctors().ToList();
            var query = doctors.AsQueryable();
            int pageNumber = 2, pageSize = 5;
            var expectedDoctors = 
                doctors.Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

            var specifications = new Specifications
            {
                PageNumber = pageNumber, 
                PageSize = pageSize
            };

            // Act
            var actualDoctors = service.ApplySpecifications(query, specifications)?.ToList();

            // Assert
            Assert.NotNull(actualDoctors);
            Assert.True(actualDoctors.Any());
            Assert.Equal(pageSize, actualDoctors.Count);
            Assert.True(expectedDoctors.SequenceEqual(actualDoctors));
        }
    }
}
