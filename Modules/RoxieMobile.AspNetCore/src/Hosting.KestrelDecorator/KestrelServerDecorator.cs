using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoxieMobile.AspNetCore.Hosting.KestrelDecorator.Resources;

// KestrelServer.cs
// @link https://github.com/dotnet/aspnetcore/blob/v3.1.1/src/Servers/Kestrel/Core/src/KestrelServer.cs

// TestServer.cs
// @link https://github.com/dotnet/aspnetcore/blob/v3.1.1/src/Hosting/TestHost/src/TestServer.cs

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public sealed class KestrelServerDecorator : IServer, ILoopbackHttpClientFactory
    {
// MARK: - Construction

        public KestrelServerDecorator(IOptions<KestrelServerOptions> options, IConnectionListenerFactory transportFactory, ILoggerFactory loggerFactory)
        {
            // Init instance
            _kestrelServer = new KestrelServer(options, transportFactory, loggerFactory);
            _testServer = new TestServer(new ServiceCollection().BuildServiceProvider(), this.Features);
        }

        ~KestrelServerDecorator()
        {
            Dispose();
        }

        public void Dispose()
        {
            var disposables = new List<IDisposable?>();
            ExecSafe(() => {

                _disposed = true;

                // Collect disposable objects
                disposables.Add(_kestrelServer);
                _kestrelServer = null;

                disposables.Add(_testServer);
                _testServer = null;

                disposables.Add(_httpClientHandler);
                _httpClientHandler = null;
            });

            // Dispose collected objects
            disposables.ForEach(o => o?.Dispose());
        }

// MARK: - Properties

        public IFeatureCollection Features =>
            ExecSafeAndReturn(() => _kestrelServer?.Features ?? new FeatureCollection());

// MARK: - Methods

        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            await ExecSafeAsync(async () => {

                if (_kestrelServer != null) {
                    await _kestrelServer.StartAsync(application, cancellationToken);
                }

                if (_testServer is IServer server) {
                    await server.StartAsync(application, cancellationToken);

                    // Create shared loopback HttpMessageHandler
                    _testServer.BaseAddress = null;
                    _httpClientHandler = _testServer.CreateHandler();
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ExecSafeAsync(async () => {

                if (_testServer is IServer server) {
                    await server.StopAsync(cancellationToken);
                }

                if (_kestrelServer != null) {
                    await _kestrelServer.StopAsync(cancellationToken);
                }
            });
        }

        public HttpClient CreateClient(Uri? baseAddress)
        {
            var handler = ExecSafeAndReturn(() => _httpClientHandler ?? throw new InvalidOperationException(Messages.ServerIsNotStarted));

            return new HttpClient(handler, disposeHandler: false) {
                BaseAddress = baseAddress
            };
        }

// MARK: - Private Methods

        private void ExecSafe(Action action)
        {
            _syncMutex.Wait();
            try {
                if (_disposed) {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                action();
            }
            finally {
                _syncMutex.Release();
            }
        }

        private async Task ExecSafeAsync(Func<Task> action)
        {
            _syncMutex.Wait();
            try {
                if (_disposed) {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                await action();
            }
            finally {
                _syncMutex.Release();
            }
        }

        private TResult ExecSafeAndReturn<TResult>(Func<TResult> action)
        {
            _syncMutex.Wait();
            try {
                if (_disposed) {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                return action();
            }
            finally {
                _syncMutex.Release();
            }
        }

// MARK: - Variables

        private readonly SemaphoreSlim _syncMutex = new SemaphoreSlim(1, 1);

        private IServer? _kestrelServer;

        private TestServer? _testServer;

        private HttpMessageHandler? _httpClientHandler;

        private bool _disposed;
    }
}
