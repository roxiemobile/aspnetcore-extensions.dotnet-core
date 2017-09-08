using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace RoxieMobile.AspNetCore.Hosting.HealthChecks
{
    public class HealthCheckStartupFilter : IStartupFilter
    {
// MARK: - Construction

        public HealthCheckStartupFilter(string path, int? port = null)
        {
            // Init instance variables
            _path = path;
            _port = port;
        }

// MARK: - Methods

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            app => {
                app.UseMiddleware<HealthCheckMiddleware>(_path, _port);
                next(app);
            };

// MARK: - Variables

        private readonly string _path;

        private readonly int? _port;
    }
}