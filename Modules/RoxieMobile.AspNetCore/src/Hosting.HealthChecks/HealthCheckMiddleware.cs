using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using RoxieMobile.AspNetCore.Net.Http.Headers;
using RoxieMobile.CSharpCommons.Extensions;

namespace RoxieMobile.AspNetCore.Hosting.HealthChecks
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class HealthCheckMiddleware
    {
// MARK: - Constants

        public HealthCheckMiddleware(RequestDelegate next, string path)
        {
            // Init instance variables
            _next = next;
            _path = path;
            _port = default(int?);
        }

        public HealthCheckMiddleware(RequestDelegate next, string path, int port)
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

                // Disable caching of the response
                context.Response.Headers.Tap(headers => {
                    headers[HttpHeaderNames.CacheControl] = new StringValues("no-cache, no-store, must-revalidate");
                    headers[HttpHeaderNames.Expires] = new StringValues("0");
                    headers[HttpHeaderNames.Pragma] = new StringValues("no-cache");
                });

                // Set status code
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