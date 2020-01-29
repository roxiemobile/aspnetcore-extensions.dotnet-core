using System;
using System.Diagnostics.CodeAnalysis;
using RoxieMobile.CSharpCommons.Diagnostics;
using RoxieMobile.CSharpCommons.Extensions;
using Serilog.Events;
using Serilog.Filters;

// ControllerActionInvoker.cs
// @link https://github.com/dotnet/aspnetcore/blob/v3.1.1/src/Mvc/Mvc.Core/src/Infrastructure/ControllerActionInvoker.cs

// MvcCoreLoggerExtensions.cs
// @link https://github.com/dotnet/aspnetcore/blob/v3.1.1/src/Mvc/Mvc.Core/src/MvcCoreLoggerExtensions.cs

namespace RoxieMobile.AspNetCore.Logging.Serilog
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Filters
    {
// MARK: - Subtypes

        public static class ByExcluding
        {
            public static Func<LogEvent, bool> ActionMethodExecuting()
            {
                return MatchingSourceWithMessage(
                    "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker",
                    "Executing action method");
            }
        }

// MARK: - Methods

        public static Func<LogEvent, bool> MatchingSourceWithMessage(string source, string? message)
        {
            Guard.NotBlank(source, Funcs.Blank(nameof(source)));

            var funcFromSource = Matching.FromSource(source);
            return e => funcFromSource(e) && (message.IsEmpty() || e.MessageTemplate.Text.StartsWith(message));
        }
    }
}
