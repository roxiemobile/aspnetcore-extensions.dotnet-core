using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
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
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseKestrelDecorator(
            this IWebHostBuilder hostBuilder,
            Action<KestrelServerOptions> options)
        {
            return hostBuilder
                .UseKestrelDecorator()
                .ConfigureServices(services => services.Configure(options));
        }
    }
}