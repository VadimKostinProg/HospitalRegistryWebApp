using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public abstract class AppointmentsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IAppointmentsService service;
    protected readonly Mock<IDoctorsService> doctorsServiceMock;
    protected readonly Mock<ISchedulesService> schedulesServiceMock;

    public AppointmentsServiceTestsBase()
    {
        doctorsServiceMock = new Mock<IDoctorsService>();
        schedulesServiceMock = new Mock<ISchedulesService>();
        service = new AppointmentsService(repositoryMock.Object, doctorsServiceMock.Object, schedulesServiceMock.Object);
    }
}