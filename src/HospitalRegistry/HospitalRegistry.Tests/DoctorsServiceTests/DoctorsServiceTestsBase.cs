using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public abstract class DoctorsServiceTestsBase : HospitalRegistryTestsBase
    {
        protected IDoctorsService service;
        protected readonly Mock<ISpecificationsService> specificationsServiceMock;

        public DoctorsServiceTestsBase()
        {
            specificationsServiceMock = new Mock<ISpecificationsService>();
            service = new DoctorsService(repositoryMock.Object, specificationsServiceMock.Object);
        }
    }
}
