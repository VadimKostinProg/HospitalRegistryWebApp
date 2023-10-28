using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public abstract class DoctorsServiceTestsBase : HospitalRegistryTestsBase
    {
        protected IDoctorsService service;

        public DoctorsServiceTestsBase()
        {
            service = new DoctorsService(repositoryMock.Object);
        }
    }
}
