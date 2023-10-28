using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;

namespace HospitalRegistry.Tests.SchedulesServiceTests;

public abstract class SchedulesServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly ISchedulesService service;

    public SchedulesServiceTestsBase()
    {
        service = new SchedulesService(repositoryMock.Object);
    }
}