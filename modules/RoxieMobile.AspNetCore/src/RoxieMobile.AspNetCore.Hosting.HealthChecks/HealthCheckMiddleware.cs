using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace RoxieMobile.AspNetCore.Hosting.HealthChecks
{
    public sealed class HealthCheckMiddleware
    {
// MARK: - Constants

        public HealthCheckMiddleware(RequestDelegate next, string path, int? port = null)
        {
            // Init instance variables
            _next = next;
            _path = path;
            _port = port;
        }

// MARK: - Methods

        public async Task Invoke(HttpContext context)
        {
            // Handle HealthCheck request
            if (IsHealthCheckRequest(context)) {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            else {
                await _next.Invoke(context);
            }
        }

// MARK: - Private Methods

        private bool IsHealthCheckRequest(HttpContext context)
        {
            var result = false;
            if (_port.HasValue) {

                var connInfo = context.Features.Get<IHttpConnectionFeature>();
                if (connInfo.LocalPort == _port) {

                    result = (context.Request.Path == _path);
                }
            }
            else {
                result = (context.Request.Path == _path);
            }

            // Done
            return result;
        }

// MARK: - Variables

        private readonly RequestDelegate _next;

        private readonly string _path;

        private readonly int? _port;
    }
}