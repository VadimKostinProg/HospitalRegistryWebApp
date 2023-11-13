using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;

namespace HospitalRegistry.Tests.AppointmentsServiceTests;

public abstract class AppointmentsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IAppointmentsService service;
    protected readonly Mock<IDoctorsService> doctorsServiceMock;
    protected readonly Mock<ISchedulesService> schedulesServiceMock;
    protected readonly Mock<ISpecificationsService> specificationsServiceMock;
    protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
    protected readonly Mock<UserManager<ApplicationUser>> userManagerMock;

    public AppointmentsServiceTestsBase()
    {
        doctorsServiceMock = new Mock<IDoctorsService>();
        schedulesServiceMock = new Mock<ISchedulesService>();
        specificationsServiceMock = new Mock<ISpecificationsService>();
        httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        userManagerMock = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
        service = new AppointmentsService(repositoryMock.Object, doctorsServiceMock.Object, 
            schedulesServiceMock.Object, specificationsServiceMock.Object, 
            httpContextAccessorMock.Object, userManagerMock.Object);
    }
}