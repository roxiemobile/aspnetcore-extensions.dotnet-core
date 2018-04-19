using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public class KestrelServerDecorator : IServer, IApplicationHttpClientFactory
    {
// MARK: - Construction

        public KestrelServerDecorator(
            IOptions<KestrelServerOptions> options,
            ITransportFactory transportFactory,
            ILoggerFactory loggerFactory)
        {
            _server = new KestrelServer(options, transportFactory, loggerFactory);
        }

        ~KestrelServerDecorator()
        {
            Dispose();
        }

        public void Dispose()
        {
            IDisposable server = null;
            IDisposable httpClientHandler = null;

            lock (_syncLock) {

                _disposed = true;

                // Release resources
                server = _server;
                _server = null;

                httpClientHandler = _httpClientHandler;
                _httpClientHandler = null;
            }

            server?.Dispose();
            httpClientHandler?.Dispose();
        }

// MARK: - Properties

        public IFeatureCollection Features => _server.Features;

// MARK: - Methods

        public async Task StartAsync<TContext>(
            IHttpApplication<TContext> application,
            CancellationToken cancellationToken)
        {
            _application = new ApplicationWrapper<HostingApplication.Context>(
                (IHttpApplication<HostingApplication.Context>) application, () => {
                    if (_disposed) {
                        throw new ObjectDisposedException(GetType().FullName);
                    }
                });

            _httpClientHandler = new ClientHandler(PathString.Empty, _application);

            await _server.StartAsync(_application, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _server.StopAsync(cancellationToken);
        }

        public HttpClient CreateClient(Uri baseAddress)
        {
            return new HttpClient(_httpClientHandler, disposeHandler: false) {
                BaseAddress = baseAddress
            };
        }

// MARK: - Inner Types

        private class ApplicationWrapper<TContext> : IHttpApplication<TContext>
        {
            public ApplicationWrapper(IHttpApplication<TContext> application, Action preProcessRequestAsync)
            {
                _application = application;
                _preProcessRequestAsync = preProcessRequestAsync;
            }

            public TContext CreateContext(IFeatureCollection contextFeatures)
            {
                return _application.CreateContext(contextFeatures);
            }

            public void DisposeContext(TContext context, Exception exception)
            {
                _application.DisposeContext(context, exception);
            }

            public Task ProcessRequestAsync(TContext context)
            {
                _preProcessRequestAsync();
                return _application.ProcessRequestAsync(context);
            }

            private readonly IHttpApplication<TContext> _application;
            private readonly Action _preProcessRequestAsync;
        }

// MARK: - Variables

        private IServer _server;

        private IHttpApplication<HostingApplication.Context> _application;

        private HttpMessageHandler _httpClientHandler;

        private bool _disposed = false;

        private static readonly object _syncLock = new object();
    }
}