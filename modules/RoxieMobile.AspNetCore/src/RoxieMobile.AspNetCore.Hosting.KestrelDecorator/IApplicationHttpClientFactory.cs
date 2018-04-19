using System;
using System.Net.Http;

namespace RoxieMobile.AspNetCore.Hosting.KestrelDecorator
{
    public interface IApplicationHttpClientFactory
    {
        HttpClient CreateClient(Uri baseAddress);
    }
}