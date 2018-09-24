using Microsoft.AspNetCore.Http;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;

namespace RoxieMobile.AspNetCore.Net.Http.Headers
{
    /// <summary>
    /// Header value for X-Powered-By header
    /// </summary>
    public class XPoweredByHeader : HeaderPolicyBase
    {
        /// <inheritdoc />
        public XPoweredByHeader(string value) : base(value)
        {
            this.Value = value;
        }

        /// <inheritdoc />
        public override string Header { get; } = "X-Powered-By";

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

        private void EvaluateRequest(HttpContext context, CustomHeadersResult result)
        {
            if (string.IsNullOrEmpty(this.Value)) {
                result.RemoveHeaders.Add(this.Header);
            }
            else {
                result.SetHeaders[this.Header] = this.Value;
            }
        }
    }
}