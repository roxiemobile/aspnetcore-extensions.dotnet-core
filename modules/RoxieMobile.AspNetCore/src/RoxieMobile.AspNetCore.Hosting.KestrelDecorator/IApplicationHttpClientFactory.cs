using System;
using System.Net.Http;

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public interface IApplicationHttpClientFactory
    {
        /// <summary>
        /// Creates a new <see cref="HttpClient"/> using the default configuration and passed base address.
        /// </summary>
        /// <param name="baseAddress">
        /// The base address of Uniform Resource Identifier (URI) of the Internet resource
        /// which will be used when sending requests. 
        /// </param>
        /// <returns>
        /// An <see cref="HttpClient"/> configured using the default configuration and passed base address.
        /// </returns>
        HttpClient CreateClient(Uri baseAddress);
    }
}