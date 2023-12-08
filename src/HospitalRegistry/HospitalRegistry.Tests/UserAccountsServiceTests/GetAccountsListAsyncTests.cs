using AutoFixture;
using FluentAssertions;
using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HospitalRegistry.Tests.UserAccountsServiceTests;

public class GetAccountsListAsyncTests : UserAccountsServiceTestsBase
{
    [Fact]
    public async Task GetAccountsListAsyncTests_SpecificationsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        AccountSpecificationsDTO specifications = null;

        // Assert
        var action = async () =>
        {
            // Act
            var list = await service.GetAccountsListAsync(specifications);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(5, 1)]
    [InlineData(4, 2)]
    [InlineData(3, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    public async Task GetAccountsListAsyncTests_ValidObject_ReturnsAccountsList(int pageSize, int pageNumber)
    {
        // Arrange
        var searchTerm = "testName";

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        };

        var applicationUsers = fixture.Build<ApplicationUser>()
            .With(x => x.UserRoles, new List<IdentityUserRole<Guid>>() 
            { 
                new IdentityUserRole<Guid>() { RoleId = role.Id } 
            })
            .CreateMany(10)
            .ToList();
        applicationUsers[0].FullName = applicationUsers[3].FullName = applicationUsers[5].FullName = searchTerm;

        var applicationUserQuery = applicationUsers.AsQueryable();
        userManagerMock.Setup(x => x.Users).Returns(applicationUserQuery);
        userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        roleManagerMock.Setup(x => x.FindByNameAsync("Admin"))
            .ReturnsAsync(role);

        var expectedAccounts = applicationUsers
            .Where(x => x.FullName == searchTerm)
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AccountResponse
            {
                Id = x.Id,
                FullName= x.FullName,
                Email = x.Email,
                Role = "Admin"
            })
            .ToList();

        var expectedTotalPages = (int)Math.Ceiling((double)expectedAccounts.Count / pageSize);


        var specifications = new AccountSpecificationsDTO
        {
            SearchTerm = searchTerm,
            Role = "Admin",
            SortField = "Id",
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        // Act
        var list = await service.GetAccountsListAsync(specifications);

        // Assert
        list.Should().NotBeNull();
        list.List.Should().BeEquivalentTo(expectedAccounts);
        list.TotalPages.Should().Be(expectedTotalPages);
    }
}
