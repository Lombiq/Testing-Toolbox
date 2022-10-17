using Lombiq.Tests.Integration.Services;
using Lombiq.Tests.Resolvers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq.AutoMock.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Moq.AutoMock;

public static class AutoMockerExtensions
{
    /// <summary>
    /// Register zero or more services with <see cref="AutoMocker.Use{TService}(TService)"/> as <see
    /// cref="IEnumerable{TService}"/>. This is to simulate the DI feature where you can inject all implementations of a
    /// given service as <see cref="IEnumerable{TService}"/>.
    /// </summary>
    /// <param name="mocker">The <see cref="AutoMocker"/> performing the registration.</param>
    /// <param name="objects">The collection of instances to be registered.</param>
    /// <typeparam name="T">The service type.</typeparam>
    public static void Some<T>(this AutoMocker mocker, params T[] objects) => mocker.Use<IEnumerable<T>>(objects);

    public static IStringLocalizer<T> MockStringLocalizer<T>(this AutoMocker mocker)
    {
        var localizerMock = mocker.GetMock<IStringLocalizer<T>>();
        localizerMock
            .Setup(localizer => localizer[It.IsAny<string>()])
            .Returns<string>(parameter => new LocalizedString(parameter, parameter));
        return localizerMock.Object;
    }

    public static void MockStringLocalization(this AutoMocker mocker)
    {
        var localizer = mocker.MockStringLocalizer<Mock>();
        var factoryMock = new Mock<IStringLocalizerFactory>();

        factoryMock
            .Setup(factory => factory.Create(It.IsAny<Type>()))
            .Returns<Type>(_ => localizer);

        factoryMock
            .Setup(factory => factory.Create(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((_, _) => localizer);

        mocker.Use(factoryMock.Object);
        mocker.Use<IStringLocalizer>(localizer);
        mocker.EnsureResolver<StringLocalizerResolver>();
    }

    /// <summary>
    /// Registers a new <see cref="LoggerFactory"/> that uses <see cref="ListLogger"/>.
    /// </summary>
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "It's up to the service provider.")]
    public static ListLoggerProvider MockLogging(this AutoMocker mocker)
    {
        var provider = new ListLoggerProvider();

        mocker.Use<ILoggerFactory>(new LoggerFactory(new ILoggerProvider[] { provider }));
        mocker.EnsureResolver<LoggerResolver>();

        return provider;
    }

    public static void EnsureResolver<TResolver>(this AutoMocker mocker)
        where TResolver : IMockResolver, new()
    {
        if (mocker.Resolvers.OfType<TResolver>().SingleOrDefault() is { }) return;
        mocker.Resolvers.Add(new TResolver());
    }
}
