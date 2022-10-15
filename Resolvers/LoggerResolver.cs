using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using System;

namespace Lombiq.Tests.Resolvers;

public class LoggerResolver : GenericTypeResolver
{
    protected override Type ExpectedType { get; } = typeof(ILogger<>);

    protected override object ResolveValue(AutoMocker mocker, Type serviceType, Type genericType)
    {
        var factory = mocker.Get<ILoggerFactory>() ?? throw new InvalidOperationException(
            $"Missing service. Please call {nameof(AutoMockerExtensions.MockLogging)}.");

        return factory.CreateLogger(genericType.Name);
    }
}

public class StringLocalizerResolver : GenericTypeResolver
{
    protected override Type ExpectedType { get; } = typeof(IStringLocalizer<>);

    protected override object ResolveValue(AutoMocker mocker, Type serviceType, Type genericType)
    {
        var factory = mocker.Get<IStringLocalizerFactory>() ?? throw new InvalidOperationException(
            $"Missing service. Please call {nameof(AutoMockerExtensions.MockStringLocalization)}.");

        return factory.Create(genericType);
    }
}
