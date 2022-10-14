using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace Lombiq.Tests.Integration.Services;

public class TestReverseProxy : IDisposable, IAsyncDisposable
{
    private bool _disposed;
    private bool _disposedAsync;
    private IWebHost _webHost;
    private IProxyConnectionProvider _proxyConnectionProvider;

    public string RootUrl { get; private set; }

    public TestReverseProxy(string rootUrl) => RootUrl = rootUrl;

    public void AttachConnectionProvider(IProxyConnectionProvider clientConnectionProvider) =>
        _proxyConnectionProvider = clientConnectionProvider;

    public void DetachConnectionProvider() =>
        _proxyConnectionProvider = null;

    public Task StartAsync()
    {
        if (_webHost != null)
        {
            throw new InvalidOperationException("The instance has already started.");
        }

        var webHostBuilder = new WebHostBuilder()
            .UseKestrel()
            .UseUrls(RootUrl)
            .ConfigureServices(services => services
                .AddHttpForwarder()
                .AddRouting())
            .Configure(builder =>
            {
                var httpForwarder = builder.ApplicationServices.GetRequiredService<IHttpForwarder>();

                builder.UseRouting()
                    .UseEndpoints(endpoints => endpoints
                        .Map("/{**catch-all}", async httpContext =>
                        {
                            using var client = new HttpMessageInvoker(
                                new TestProxyMessageHandler(_proxyConnectionProvider.CreateClient()));
                            await httpForwarder.SendAsync(
                                httpContext,
                                _proxyConnectionProvider.BaseAddress.ToString(),
                                client);
                        }));
            });

        _webHost = webHostBuilder.Build();

        return _webHost.StartAsync();
    }

    public Task StopAsync()
    {
        if (_webHost == null)
        {
            throw new InvalidOperationException("The instance has not been started.");
        }

        return StopInternalAsync();
    }

    private async Task StopInternalAsync()
    {
        await _webHost.StopAsync();
        _webHost.Dispose();
        _webHost = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && !_disposedAsync)
            {
                DisposeAsync()
                    .AsTask()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed || _disposedAsync) return;

        if (_webHost != null) await StopAsync().ConfigureAwait(false);

        _disposedAsync = true;

        Dispose();
        GC.SuppressFinalize(this);
    }

    // This is required because HttpClient instance is not usable directly because of performance issues. Explained here:
    // https://github.com/microsoft/reverse-proxy/blob/92370b140092e852745e98fbc33987da57b723b2/src/ReverseProxy/Forwarder/HttpForwarder.cs#L97
    internal class TestProxyMessageHandler : HttpMessageHandler
    {
        private HttpClient _client;

        public TestProxyMessageHandler(HttpClient client) => _client = client;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => _client.SendAsync(request, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            _client?.Dispose();
            _client = null;

            base.Dispose(disposing);
        }
    }
}
