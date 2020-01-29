using System;
using System.Net.Http;

// IHttpClientFactory.cs
// @link https://github.com/dotnet/extensions/blob/v3.1.1/src/HttpClientFactory/Http/src/IHttpClientFactory.cs

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    /// <summary>
    /// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with a given base address.
    /// </summary>
    /// <remarks>
    /// The <see cref="ILoopbackHttpClientFactory"/> will be registered in the service collection as a singleton.
    /// </remarks>
    public interface ILoopbackHttpClientFactory
    {
        /// <summary>
        /// Creates and configures an <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="baseAddress">The base address of the client to create.</param>
        /// <returns>A new <see cref="HttpClient"/> instance.</returns>
        /// <remarks>
        /// <para>
        /// Each call to <see cref="CreateClient(Uri)"/> is guaranteed to return a new <see cref="HttpClient"/>
        /// instance. Callers may cache the returned <see cref="HttpClient"/> instance indefinitely or surround
        /// its use in a <langword>using</langword> block to dispose it when desired.
        /// </para>
        /// <para>
        /// Callers are also free to mutate the returned <see cref="HttpClient"/> instance's public properties
        /// as desired.
        /// </para>
        /// </remarks>
        HttpClient CreateClient(Uri? baseAddress);
    }
}
