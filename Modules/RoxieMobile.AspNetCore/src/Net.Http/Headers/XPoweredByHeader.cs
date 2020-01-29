using Microsoft.AspNetCore.Http;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using RoxieMobile.CSharpCommons.Extensions;

namespace RoxieMobile.AspNetCore.Net.Http.Headers
{
    /// <summary>
    /// Header value for X-Powered-By header
    /// </summary>
    public sealed class XPoweredByHeader : CustomHeader
    {
        /// <inheritdoc />
        public XPoweredByHeader(string value) :
            base(HttpHeaderNames.XPoweredBy, value)
        {}

        /// <inheritdoc />
        public override string Header { get; } = HttpHeaderNames.XPoweredBy;

        /// <summary>
        /// Removes the X-Powered-By header from the response
        /// </summary>
        /// <returns></returns>
        public static XPoweredByHeader Remove() =>
            new XPoweredByHeader(string.Empty);

        /// <inheritdoc />
        protected override void EvaluateHttpRequest(HttpContext context, CustomHeadersResult result) =>
            EvaluateRequest(context, result);

        /// <inheritdoc />
        protected override void EvaluateHttpsRequest(HttpContext context, CustomHeadersResult result) =>
            EvaluateRequest(context, result);

        private void EvaluateRequest(HttpContext context, CustomHeadersResult result)
        {
            var value = GetValue(context);
            if (value.IsEmpty()) {
                result.RemoveHeaders.Add(this.Header);
            }
            else {
                result.SetHeaders[this.Header] = value;
            }
        }
    }
}
