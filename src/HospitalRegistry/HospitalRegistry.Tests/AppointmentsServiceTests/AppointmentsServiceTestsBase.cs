using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public abstract class AppointmentsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IAppointmentsService service;
    protected readonly Mock<IDoctorsService> doctorsServiceMock;
    protected readonly Mock<ISchedulesService> schedulesServiceMock;
    protected readonly Mock<ISpecificationsService> specificationsServiceMock;

    public AppointmentsServiceTestsBase()
    {
        doctorsServiceMock = new Mock<IDoctorsService>();
        schedulesServiceMock = new Mock<ISchedulesService>();
        specificationsServiceMock = new Mock<ISpecificationsService>();
        service = new AppointmentsService(repositoryMock.Object, doctorsServiceMock.Object, 
            schedulesServiceMock.Object, specificationsServiceMock.Object);
    }
}