using Microsoft.AspNetCore.Builder;

namespace RoxieMobile.AspNetCore.Net.Http.Headers
{
    /// <summary>
    /// Extension methods for adding a <see cref="XPoweredByHeader" /> to a <see cref="HeaderPolicyCollection" />
    /// </summary>
    public static class XPoweredByHeaderExtensions
    {
        /// <summary>
        /// Removes the X-Powered-By header from all responses
        /// </summary>
        /// <param name="policies">The collection of policies</param>
        public static HeaderPolicyCollection RemoveXPoweredByHeader(this HeaderPolicyCollection policies)
        {
            var policy = XPoweredByHeader.Remove();
            policies[policy.Header] = policy;
            return policies;
        }
    }
}