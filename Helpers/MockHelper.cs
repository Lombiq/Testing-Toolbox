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

        /// <summary>
        /// Creates an <see cref="AutoMocker"/> and resolves an instance of <typeparamref name="T"/> from it.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="configurator">
        /// Delegate to apply configuration to the <see cref="AutoMocker"/> instance. Optional, defaults to
        /// <see langword="null"/>.
        /// </param>
        /// <param name="enablePrivate">
        /// When <see langword="true"/>, non-public constructors will also be used to create mocks. Optional, defaults
        /// to <see langword="false"/>.
        /// </param>
        public static T CreateAutoMockerInstance<T>(Action<AutoMocker> configurator = null, bool enablePrivate = false)
            where T : class =>
            CreateAutoMockerInstance<T>(configurator, enablePrivate, out _);

        /// <summary>
        /// Creates an <see cref="AutoMocker"/> and resolves an instance of <typeparamref name="T"/> from it.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="mocker">The newly created <see cref="AutoMocker"/> instance.</param>
        /// <param name="enablePrivate">
        /// When <see langword="true"/>, non-public constructors will also be used to create mocks. Optional, defaults
        /// to <see langword="false"/>.
        /// </param>
        public static T CreateAutoMockerInstance<T>(out AutoMocker mocker, bool enablePrivate = false)
            where T : class =>
            CreateAutoMockerInstance<T>(null, enablePrivate, out mocker);

        /// <summary>
        /// Creates an <see cref="AutoMocker"/> and resolves an instance of <typeparamref name="T"/> from it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that configurator is also needed because if you want control on what exactly will be injected into the
        /// created instance you'll need to set that up before instantiation. Then the AutoMocker instance needs to be
        /// available afterwards too, at least for verification.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="configurator">Delegate to apply configuration to the <see cref="AutoMocker"/> instance.</param>
        /// <param name="mocker">The newly created <see cref="AutoMocker"/> instance.</param>
        /// <param name="enablePrivate">
        /// When <see langword="true"/>, non-public constructors will also be used to create mocks.
        /// </param>
        public static T CreateAutoMockerInstance<T>(Action<AutoMocker> configurator, bool enablePrivate, out AutoMocker mocker)
            where T : class
        {
            mocker = new AutoMocker();
            configurator?.Invoke(mocker);
            return mocker.CreateInstance<T>(enablePrivate);
        }
    }
}
