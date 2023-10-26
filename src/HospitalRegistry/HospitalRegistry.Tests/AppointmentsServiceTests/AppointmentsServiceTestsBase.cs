using AutoFixture;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public abstract class AppointmentsServiceTestsBase
{
    protected readonly IAppointmentsService service;
    protected readonly Mock<IAsyncRepository> repositoryMock;
    protected readonly Fixture fixture;

    public AppointmentsServiceTestsBase()
    {
        fixture = new Fixture();

        repositoryMock = new Mock<IAsyncRepository>();

        service = new AppointmentsService(repositoryMock.Object, null);
    }
}