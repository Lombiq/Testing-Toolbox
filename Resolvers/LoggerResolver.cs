using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using System;

namespace Lombiq.Tests.Resolvers;

public class LoggerResolver : GenericTypeResolver
{
    protected override Type ExpectedType { get; } = typeof(ILogger<>);

    protected override object ResolveValue(AutoMocker mocker, Type serviceType, Type genericType)
    {
        var factory = mocker.Get<LoggerFactory>();
        if (factory == null)
        {
            mocker.MockLogging();
            factory = mocker.Get<LoggerFactory>() ?? throw new InvalidOperationException("Couldn't mock logging!");
        }

        return factory.CreateLogger(genericType.Name);
    }
}
