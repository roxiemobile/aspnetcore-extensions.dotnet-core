using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using RoxieMobile.CSharpCommons.Extensions;

namespace RoxieMobile.AspNetCore.Net.Http.Headers
{
    /// <summary>
    /// Header value for X-Powered-By header
    /// </summary>
    public sealed class XPoweredByHeader : HeaderPolicyBase
    {
        /// <inheritdoc />
        public XPoweredByHeader(string value) : base(value)
        {
            this.Value = value;
        }

        /// <inheritdoc />
        public override string Header { get; } = HttpHeaderNames.XPoweredBy;

        /// <summary>
        /// Removes the X-Powered-By header from the response
        /// </summary>
        /// <returns></returns>
        public static XPoweredByHeader Remove()
        {
            return new XPoweredByHeader(null);
        }

        /// <inheritdoc />
        protected override void EvaluateHttpRequest(HttpContext context, CustomHeadersResult result)
        {
            EvaluateRequest(context, result);
        }

        /// <inheritdoc />
        protected override void EvaluateHttpsRequest(HttpContext context, CustomHeadersResult result)
        {
            EvaluateRequest(context, result);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void EvaluateRequest(HttpContext context, CustomHeadersResult result)
        {
            if (this.Value.IsEmpty()) {
                result.RemoveHeaders.Add(this.Header);
            }
            else {
                result.SetHeaders[this.Header] = this.Value;
            }
        }
    }
}