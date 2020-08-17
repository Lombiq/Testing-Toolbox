using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Lombiq.Tests.Helpers
{
    public static class MockHelper
    {
        public static ControllerContext CreateMockControllerContextWithUser() =>
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() },
            };

        public static void ConfigureMockAutherizationService(this AutoMocker mocker, AuthorizationResult authorizationResult) =>
            mocker
                .GetMock<IAuthorizationService>()
                .Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(authorizationResult);

        public static T CreateAutoMockerInstance<T>(Action<AutoMocker> configurator = null)
where T : class =>
            CreateAutoMockerInstance<T>(configurator, out _);

        public static T CreateAutoMockerInstance<T>(out AutoMocker mocker)
where T : class =>
            CreateAutoMockerInstance<T>(null, out mocker);

        // Note that configurator is also needed because if you want control on what exactly will be injected into the
        // created instance you'll need to set that up before instantiation. Then the AutoMocker instance needs to be
        // available afterwards too, at least for verification.
        public static T CreateAutoMockerInstance<T>(Action<AutoMocker> configurator, out AutoMocker mocker)
            where T : class
        {
            mocker = new AutoMocker();
            configurator?.Invoke(mocker);
            return mocker.CreateInstance<T>();
        }
    }
}
