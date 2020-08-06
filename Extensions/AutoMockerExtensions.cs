using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Moq.AutoMock
{
    public static class AutoMockerExtensions
    {
        /// <summary>
        /// Register zero or more services with <see cref="AutoMocker.Use{TService}(TService)"/> as
        /// <see cref="IEnumerable{TService}"/>. This is to simulate the DI feature where you can inject all
        /// implementations of a given service as <see cref="IEnumerable{TService}"/>.
        /// </summary>
        /// <param name="mocker">The <see cref="AutoMocker"/> performing the registration.</param>
        /// <param name="objects">The collection of instances to be registered</param>
        /// <typeparam name="T">The service type.</typeparam>
        public static void Some<T>(this AutoMocker mocker, params T[] objects) => mocker.Use<IEnumerable<T>>(objects);

        public static void MockStringLocalizer<T>(this AutoMocker mocker) =>
            mocker.GetMock<IStringLocalizer<T>>()
                .Setup(localizer => localizer[It.IsAny<string>()])
                .Returns<string>(parameter => new LocalizedString(parameter, parameter));
    }
}
