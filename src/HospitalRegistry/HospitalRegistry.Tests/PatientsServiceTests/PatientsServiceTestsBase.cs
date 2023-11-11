using AutoFixture;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public abstract class PatientsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IPatientsService service;
    protected readonly Mock<ISpecificationsService> specificationsServiceMock;

    public PatientsServiceTestsBase()
    {
        specificationsServiceMock = new Mock<ISpecificationsService>();
        service = new PatientsService(repositoryMock.Object, specificationsServiceMock.Object);
    }
}