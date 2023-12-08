using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HospitalRegistry.Tests.UserAccountsServiceTests;

public abstract class UserAccountsServiceTestsBase : HospitalRegistryTestsBase
{
    protected readonly IUserAccountsService service;
    protected readonly Mock<UserManager<ApplicationUser>> userManagerMock;
    protected readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
    protected readonly Mock<RoleManager<ApplicationRole>> roleManagerMock;
    protected readonly Mock<IJwtService> jwtServiceMock;

    public UserAccountsServiceTestsBase()
    {
        userManagerMock = this.MockUserManager();
        signInManagerMock = this.MockSignInManager(userManagerMock.Object);
        roleManagerMock = this.MockRoleManager();
        jwtServiceMock = new Mock<IJwtService>();

        service = new UserAccountsService(userManagerMock.Object, signInManagerMock.Object, roleManagerMock.Object,
            jwtServiceMock.Object, repositoryMock.Object);
    }

    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

        return mgr;
    }

    private Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        var claimFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var mgr = new Mock<SignInManager<ApplicationUser>>(userManager, accessorMock.Object, claimFactoryMock.Object,
            null, null, null, null);

        return mgr;
    }

    private Mock<RoleManager<ApplicationRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<ApplicationRole>>();
        var mgr = new Mock<RoleManager<ApplicationRole>>(store.Object, null, null, null, null);

        return mgr;
    }
}
