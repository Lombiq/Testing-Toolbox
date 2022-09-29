using System;
using System.Net.Http;

namespace Lombiq.Tests.Integration.Services;

/// <summary>
/// Represents an object that can resolve the required values for the <see cref="TestReverseProxy"/> instance.
/// </summary>
public interface IProxyConnectionProvider
{
    /// <summary>
    /// Gets the url prefix for where to forward the request to.
    /// </summary>
    Uri BaseAddress { get; }

    /// <summary>
    /// Creates the HTTP client used to forward the request to.
    /// </summary>
    HttpClient CreateClient();
}
