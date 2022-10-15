using Lombiq.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using Moq.AutoMock;

namespace Lombiq.Tests.Integration.Controllers;

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
}
