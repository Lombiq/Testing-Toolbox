using Lombiq.Tests.Integration.Services;
using Lombiq.Tests.Resolvers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq.AutoMock.Resolvers;
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

    public static void MockStringLocalizer<T>(this AutoMocker mocker) =>
        mocker.GetMock<IStringLocalizer<T>>()
            .Setup(localizer => localizer[It.IsAny<string>()])
            .Returns<string>(parameter => new LocalizedString(parameter, parameter));

    /// <summary>
    /// Registers a new <see cref="LoggerFactory"/> that uses <see cref="ListLogger"/>.
    /// </summary>
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "It's up to the service provider.")]
    public static void MockLogging(this AutoMocker mocker)
    {
        mocker.Use(new LoggerFactory(new[] { new ListLoggerProvider() }));
        mocker.EnsureResolver<LoggerResolver>();
    }

    public static void MockStringLocalization(this AutoMocker mocker)
    {
        var factory = new Mock<IStringLocalizerFactory>();
        factory.Setup()
        mocker.Use(new StringLo(new[] { new ListLoggerProvider() }));
        mocker.EnsureResolver<LoggerResolver>();
    }

    public static void EnsureResolver<TResolver>(this AutoMocker mocker)
        where TResolver : IMockResolver, new()
    {
        if (mocker.Resolvers.OfType<TResolver>().SingleOrDefault() is { }) return;
        mocker.Resolvers.Add(new TResolver());
    }
}
