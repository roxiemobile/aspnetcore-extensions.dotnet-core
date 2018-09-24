using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RoxieMobile.CSharpCommons.Diagnostics;

namespace RoxieMobile.AspNetCore.Hosting.HealthChecks
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseHealthChecks(this IWebHostBuilder builder, string path, int? port = null)
        {
            var names = (Path: nameof(path), Port: nameof(port));
            Guard.NotNull(path, Funcs.Null(names.Path));

            Guard.True(!path.Contains("?"),
                () => $"‘{names.Path}’ cannot contain query string values.");
            Guard.True(path.StartsWith("/"),
                () => $"‘{names.Path}’ should start with ‘/’.");
            Guard.True(!port.HasValue || (port > 0 && port < 65536),
                () => $"‘{names.Port}’ must be a value between 1 and 65535.");

            builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(
                new HealthCheckStartupFilter(path, port)));
            return builder;
        }
    }
}