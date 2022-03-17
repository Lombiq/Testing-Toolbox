using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.Tests.Helpers;

// Copy-pasted from https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Shared/MockHelpers.cs to be able
// to test UserGroupService. Also see:
// https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
public static class UserManagerMockHelpers
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>()
        where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var userManagerMock = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        userManagerMock.Object.UserValidators.Add(new UserValidator<TUser>());
        userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return userManagerMock;
    }

    public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null)
        where TRole : class
    {
        store ??= new Mock<IRoleStore<TRole>>().Object;
        var roles = new List<IRoleValidator<TRole>> { new RoleValidator<TRole>() };
        return new Mock<RoleManager<TRole>>(
            store,
            roles,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null);
    }

    public static Mock<IOptions<IdentityOptions>> MockIdentityOptions()
    {
        var options = new Mock<IOptions<IdentityOptions>>();
        var idOptions = new IdentityOptions { Lockout = { AllowedForNewUsers = false } };
        options.Setup(o => o.Value).Returns(idOptions);
        return options;
    }

    public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null)
        where TUser : class
    {
        store ??= new Mock<IUserStore<TUser>>().Object;
        var options = MockIdentityOptions();
        var userValidators = new List<IUserValidator<TUser>>();
        var validator = new Mock<IUserValidator<TUser>>();
        userValidators.Add(validator.Object);
        var pwdValidators = new List<PasswordValidator<TUser>> { new PasswordValidator<TUser>() };
        var userManager = new UserManager<TUser>(
            store,
            options.Object,
            new PasswordHasher<TUser>(),
            userValidators,
            pwdValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            services: null,
            new Mock<ILogger<UserManager<TUser>>>().Object);
        validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
        return userManager;
    }

    public static RoleManager<TRole> TestRoleManager<TRole>(IRoleStore<TRole> store = null)
        where TRole : class
    {
        store ??= new Mock<IRoleStore<TRole>>().Object;
        var roles = new List<IRoleValidator<TRole>> { new RoleValidator<TRole>() };
        return new RoleManager<TRole>(
            store,
            roles,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            logger: null);
    }
}
