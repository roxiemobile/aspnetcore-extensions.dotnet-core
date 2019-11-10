using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoxieMobile.CSharpCommons.Diagnostics;

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public static class WebHostBuilderKestrelExtensions
    {
        /// <summary>
        /// Specify KestrelServerDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The <see cref="IWebHostBuilder"/> to configure.
        /// </param>
        /// <returns>
        /// The <see cref="IWebHostBuilder"/>.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(
            this IWebHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseKestrel()
                .ConfigureServices(services => {
                    // Remove KestrelServer instance from services
                    services.RemoveAll<IServer>();
                    // Add KestrelServerDecorator instance as replacement of KestrelServer instance
                    services.AddSingleton<IServer, KestrelServerDecorator>();
                    services.AddSingleton(provider => provider.GetService<IServer>() as IApplicationHttpClientFactory);
                });
        }

        /// <summary>
        /// Specify KestrelServerDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The <see cref="IWebHostBuilder"/> to configure.
        /// </param>
        /// <param name="options">
        /// A callback to configure Kestrel options.
        /// </param>
        /// <returns>
        /// The <see cref="IWebHostBuilder"/>.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(
            this IWebHostBuilder hostBuilder,
            Action<KestrelServerOptions> options)
        {
            return hostBuilder
                .UseKestrelDecorator()
                .ConfigureServices(services => services.Configure(options));
        }

        /// <summary>
        /// Specify KestrelServerDecorator as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The <see cref="IWebHostBuilder"/> to configure.
        /// </param>
        /// <param name="configureOptions">
        /// A callback to configure Kestrel options.
        /// </param>
        /// <returns>
        /// The <see cref="IWebHostBuilder"/>.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(
            this IWebHostBuilder hostBuilder,
            Action<WebHostBuilderContext, KestrelServerOptions> configureOptions)
        {
            Guard.NotNull(configureOptions, Funcs.Null(nameof(configureOptions)));

            return hostBuilder
                .UseKestrelDecorator()
                .ConfigureServices((builderContext, services) => {
                    services.Configure<KestrelServerOptions>(options => {
                        configureOptions(builderContext, options);
                    });
                });
        }
    }
}