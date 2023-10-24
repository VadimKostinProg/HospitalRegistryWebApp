using AutoFixture;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public abstract class DoctorsServiceTestsBase
    {
        protected IDoctorsService service;
        protected Mock<IAsyncRepository> repositoryMock;
        protected Fixture fixture;

        public DoctorsServiceTestsBase()
        {
            fixture = new Fixture();

            repositoryMock = new Mock<IAsyncRepository>();

            service = new DoctorsService(repositoryMock.Object);
        }

        public IEnumerable<Doctor> GetTestDoctors(int count = 10)
        {
            for (int i = 0; i < count; i++)
                yield return GetTestDoctor();
        }

        public Doctor GetTestDoctor()
        {
            return new Doctor
            {
                Id = Guid.NewGuid(),
                Name = fixture.Create<string>(),
                Surname = fixture.Create<string>(),
                Patronymic = fixture.Create<string>(),
                DateOfBirth = "2000.01.01",
                Specialty = fixture.Create<Specialty>().ToString(),
                Email = fixture.Create<string>(),
                PhoneNumber = fixture.Create<string>(),
            };
        }
    }
}
