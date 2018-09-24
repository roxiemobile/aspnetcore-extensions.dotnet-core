using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RoxieMobile.AspNetCore.Net.Http.Headers;

namespace RoxieMobile.AspNetCore.Net.Http.Infrastructure
{
    public static class UserAgentTools
    {
        public static UserAgent DetectUserAgent(HttpContext context, string product)
        {
            foreach (var headerName in UserAgentHeaders)
            {
                if (!context.Request.Headers.ContainsKey(headerName))
                {
                    continue;
                }

                var userAgentComponents = ParseUserAgentString(context.Request.Headers[headerName]);

                var component = userAgentComponents.FirstOrDefault(c => c.Product.Equals(product, StringComparison.OrdinalIgnoreCase));
                if (component != null)
                {
                    return ToUserAgent(component);
                }
            }

            return UserAgent.Unknown;
        }

        public static IEnumerable<UserAgent> DetectUserAgents(HttpContext context)
        {
            foreach (var headerName in UserAgentHeaders)
            {
                if (!context.Request.Headers.ContainsKey(headerName))
                {
                    continue;
                }

                var userAgentComponents = ParseUserAgentString(context.Request.Headers[headerName]);

                foreach (var component in userAgentComponents)
                {
                    if (component == null) continue;
                    yield return ToUserAgent(component);
                }
            }
        }

        public static UserAgent ToUserAgent(UserAgentComponent component)
        {
            var userAgent = new UserAgent
            {
                Product = component.Product,
                Version = component.Version,
            };
            if (!string.IsNullOrWhiteSpace(component.Comment))
            {
                var commentComponents = ParseUserAgentComment(component.Comment);
                if (commentComponents.Length > 0)
                {
                    userAgent.Platform = commentComponents[0].Trim();
                }

                if (commentComponents.Length > 1)
                {
                    userAgent.PlatformVersion = commentComponents[1].Trim();
                }

                if (commentComponents.Length > 2)
                {
                    userAgent.Properties.Add("HwModel", commentComponents[2]);
                }
            }
            return userAgent;
        }

        public static IEnumerable<UserAgentComponent> ParseUserAgentString(string userAgentString)
        {
            var userAgentComponents = new List<UserAgentComponent>();
            UserAgentComponent userAgentComponent = null;

            userAgentString = userAgentString.Trim();

            while (!string.IsNullOrWhiteSpace(userAgentString))
            {
                var currentFragment = userAgentString.StartsWith("(")
                    ? UserAgentStringFragment.Comment
                    : UserAgentStringFragment.Product;

                switch (currentFragment)
                {
                    case UserAgentStringFragment.Product:
                        if (userAgentComponent == null
                            || userAgentComponent.Product != null
                            || userAgentComponent.Version != null
                            || userAgentComponent.Comment != null)
                        {
                            if (userAgentComponent != null)
                            {
                                userAgentComponents.Add(userAgentComponent);
                            }

                            userAgentComponent = new UserAgentComponent();
                        }

                        string product;
                        var productVersion = string.Empty;
                        var delimiterPos = userAgentString.IndexOf(" ", StringComparison.Ordinal);
                        var openBracketPosition = userAgentString.IndexOf("(", StringComparison.Ordinal);
                        delimiterPos = delimiterPos < 0
                            ? openBracketPosition
                            : openBracketPosition < 0
                                ? delimiterPos
                                : Math.Min(delimiterPos, openBracketPosition);

                        if (delimiterPos >= 0)
                        {
                            product = userAgentString.Substring(0, delimiterPos);
                            userAgentString = userAgentString.Substring(delimiterPos).TrimStart();
                        }
                        else
                        {
                            product = userAgentString;
                            userAgentString = string.Empty;
                        }

                        var slashPosition = product.IndexOf("/", StringComparison.Ordinal);
                        if (slashPosition >= 0)
                        {
                            productVersion = product.Substring(slashPosition + 1);
                            product = product.Substring(0, slashPosition);
                        }

                        userAgentComponent.Product = product;
                        userAgentComponent.Version = productVersion;

                        break;
                    case UserAgentStringFragment.Comment:
                        userAgentString = userAgentString.Substring(1);
                        if (userAgentComponent == null
                            || userAgentComponent.Comment != null)
                        {
                            if (userAgentComponent != null)
                            {
                                userAgentComponents.Add(userAgentComponent);
                            }

                            userAgentComponent = new UserAgentComponent();
                        }
                        var bracketIndex = 1;
                        var closingBracketPosition = -1;
                        for (int i = 0; i < userAgentString.Length; i++)
                        {
                            if (userAgentString[i] == '(') bracketIndex++;
                            if (userAgentString[i] == ')') bracketIndex--;
                            if (bracketIndex == 0)
                            {
                                closingBracketPosition = i;
                                break;
                            }
                        }
                        if (closingBracketPosition >= 0)
                        {
                            var comment = userAgentString.Substring(0, closingBracketPosition);
                            userAgentString = userAgentString.Substring(closingBracketPosition + 1).TrimStart();
                            userAgentComponent.Comment = comment;
                        }
                        else
                        {
                            return new List<UserAgentComponent>();
                        }

                        break;
                }
            }

            if (userAgentComponent != null)
            {
                userAgentComponents.Add(userAgentComponent);
            }

            return userAgentComponents;
        }

        public static string[] ParseUserAgentComment(string comment)
        {
            return comment.Split(';').Select(s => s.Trim()).ToArray();
        }

// MARK: - Constants

        private static readonly string[] UserAgentHeaders = {
            HttpHeaderNames.XDeviceUserAgent,
            HttpHeaderNames.XOriginalUserAgent,
            HttpHeaderNames.UserAgent
        };
    }
}