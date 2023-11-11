using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public abstract class DiagnosesServiceTestsBase : HospitalRegistryTestsBase
{
    protected IDiagnosesService service;
    protected readonly Mock<ISpecificationsService> specificationsServiceMock;

    public DiagnosesServiceTestsBase()
    {
        specificationsServiceMock = new Mock<ISpecificationsService>();
        service = new DiagnosesService(repositoryMock.Object, specificationsServiceMock.Object);
    }
}