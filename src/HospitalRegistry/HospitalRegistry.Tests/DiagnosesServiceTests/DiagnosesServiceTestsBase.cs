using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public abstract class DiagnosesServiceTestsBase : HospitalRegistryTestsBase
{
    protected IDiagnosesService service;

    public DiagnosesServiceTestsBase()
    {
        service = new DiagnosesService(repositoryMock.Object);
    }
}