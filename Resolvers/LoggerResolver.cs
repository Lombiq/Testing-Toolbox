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
        var factory = mocker.Get<ILoggerFactory>();
        if (factory == null)
        {
            mocker.MockLogging();
            factory = mocker.Get<ILoggerFactory>() ?? throw new InvalidOperationException("Couldn't mock logging!");
        }

        return factory.CreateLogger(genericType.Name);
    }
}

public class StringLocalizerResolver : GenericTypeResolver
{
    protected override Type ExpectedType { get; } = typeof(IStringLocalizer<>);

    protected override object ResolveValue(AutoMocker mocker, Type serviceType, Type genericType)
    {
        var factory = mocker.Get<IStringLocalizerFactory>();
        if (factory == null)
        {
            mocker.MockStringLocalization();
            factory = mocker.Get<IStringLocalizerFactory>() ??
                throw new InvalidOperationException("Couldn't mock string localization!");
        }

        return factory.Create(genericType);
    }
}
