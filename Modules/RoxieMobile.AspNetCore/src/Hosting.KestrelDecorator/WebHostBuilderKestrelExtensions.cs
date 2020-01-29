using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// WebHostBuilderKestrelExtensions.cs
// @link https://github.com/dotnet/aspnetcore/blob/v3.1.1/src/Servers/Kestrel/Kestrel/src/WebHostBuilderKestrelExtensions.cs

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public static class WebHostBuilderKestrelExtensions
    {
        /// <summary>
        /// Specify KestrelDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.UseKestrel().ConfigureServices(services =>
            {
//                // Don't override an already-configured transport
//                services.TryAddSingleton<IConnectionListenerFactory, SocketTransportFactory>();
//
//                services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
//                services.AddSingleton<IServer, KestrelServer>();

                // Remove KestrelServer instance from services
                services.RemoveAll<IServer>();

                // Add KestrelServerDecorator instance as replacement of KestrelServer instance
                services.AddSingleton<IServer, KestrelServerDecorator>();
                services.AddSingleton(provider => provider.GetService<IServer>() as ILoopbackHttpClientFactory);
            });
        }

        /// <summary>
        /// Specify KestrelDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <param name="options">
        /// A callback to configure Kestrel options.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(this IWebHostBuilder hostBuilder, Action<KestrelServerOptions> options)
        {
            return hostBuilder.UseKestrelDecorator().ConfigureKestrel(options);
        }

        /// <summary>
        /// Specify KestrelDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <param name="configureOptions">A callback to configure Kestrel options.</param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(this IWebHostBuilder hostBuilder, Action<WebHostBuilderContext, KestrelServerOptions> configureOptions)
        {
            return hostBuilder.UseKestrelDecorator().ConfigureKestrel(configureOptions);
        }
    }
}
