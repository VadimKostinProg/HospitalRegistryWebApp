using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;

namespace HospitalRegistry.Tests.SpecificationsServiceTests
{
    public abstract class SpecificationsServiceTestsBase : HospitalRegistryTestsBase
    {
        protected readonly ISpecificationsService service;

        public SpecificationsServiceTestsBase()
        {
            service = new SpecificationsService();
        }
    }
}
