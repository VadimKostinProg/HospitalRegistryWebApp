using AutoFixture;
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
    }
}
