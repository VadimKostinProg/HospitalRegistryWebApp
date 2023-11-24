using AutoFixture;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public abstract class PatientsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IPatientsService service;

    public PatientsServiceTestsBase()
    {
        service = new PatientsService(repositoryMock.Object);
    }
}