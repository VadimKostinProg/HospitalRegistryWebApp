using AutoFixture;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class GetBySpecialtyAsyncTests : DoctorsServiceTestsBase
    {
        public GetBySpecialtyAsyncTests() : base() { }

        [Theory]
        [InlineData(Specialty.Allergist)]
        [InlineData(Specialty.Urologist)]
        [InlineData(Specialty.Cardiologist)]
        [InlineData(Specialty.Surgeon)]
        [InlineData(Specialty.Radiologist)]
        public async Task GetBySpecialtyAsync_SpecialtyPassed_ReturnsFilteredDoctors(Specialty specialty)
        {
            // Arrange
            var doctors = fixture.CreateMany<Doctor>(50).AsQueryable();
            Expression<Func<Doctor, bool>> expression = (Doctor doctor) => doctor.Specialty == specialty.ToString();
            var filteredDoctors = doctors.Where(expression).ToList();
            repositoryMock.Setup(x => x.GetFilteredAsync(expression, true)).ReturnsAsync(filteredDoctors);
            var expected = filteredDoctors.Select(x => x.ToDoctorResponse()).ToList();

            // Act
            var actual = await service.GetBySpecialtyAsync(specialty);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}
