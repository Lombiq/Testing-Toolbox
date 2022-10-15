using Lombiq.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using Moq.AutoMock;
using System;

namespace Lombiq.Tests.Integration.Controllers;

/// <summary>
/// A controller or base class for testing features on controllers. Its HTTP context, host environment and service
/// providers are initialized during construction.
/// </summary>
public class AutoMockerController : Controller
{
    public string Environment { get; set; } = Environments.Development;

    public AutoMockerController(AutoMocker mocker)
    {
        mocker.Use(this);

        var hostEnvironmentMock = new Mock<IHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(hostEnvironment => hostEnvironment.EnvironmentName)
            .Returns(() => Environment);
        mocker.Use(hostEnvironmentMock.Object);

        ControllerContext = MockHelper.CreateMockControllerContextWithUser(mocker);
    }

    public void UseProductionEnvironment() => Environment = Environments.Production;

    public void SetRequestUri(Uri uri)
    {
        HttpContext.Request.Host = HostString.FromUriComponent(uri);
        HttpContext.Request.IsHttps = uri.Scheme.ToUpperInvariant() == "HTTPS";

        if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath != "/")
        {
            HttpContext.Request.Path = uri.AbsolutePath;
        }
    }
}
