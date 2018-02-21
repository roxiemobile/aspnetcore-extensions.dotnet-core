using System;
using System.Diagnostics.CodeAnalysis;
using RoxieMobile.CSharpCommons.Extensions;
using RoxieMobile.CSharpCommons.Logging;
using Serilog;
using Serilog.Events;

namespace RoxieMobile.AspNetCore.Logging.Serilog
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public sealed class SerilogLogger : Logger.IContract
    {
// MARK: - Construction

        public SerilogLogger(ILogger logger)
        {
            // Init instance variables
            _logger = logger;
            this.MinimumLogLevel = FindMinimumLogLevel(logger);
        }

// MARK: - Properties

        public Logger.LogLevel MinimumLogLevel { get; }

// MARK: - Methods

        public void V(string tag, string msg)
        {
            if (tag != null && msg != null) {
                _logger.Verbose(tag.Trim() + (tag.IsBlank() ? string.Empty : ": ") + msg);
            }
        }

        public void D(string tag, string msg)
        {
            if (tag != null && msg != null) {
                _logger.Debug(msg);
            }
        }

        public void I(string tag, string msg)
        {
            if (tag != null && msg != null) {
                _logger.Information(msg);
            }
        }

        public void W(string tag, string msg)
        {
            if (tag != null && msg != null) {
                _logger.Warning(msg);
            }
        }

        public void W(string tag, string msg, Exception exc)
        {
            if (tag != null && msg != null) {
                _logger.Warning(exc, msg);
            }
        }

        public void W(string tag, Exception exc)
        {
            if (tag != null) {
                _logger.Warning(exc, "");
            }
        }

        public void E(string tag, string msg)
        {
            if (tag != null && msg != null) {
                _logger.Error(msg);
            }
        }

        public void E(string tag, string msg, Exception exc)
        {
            if (tag != null && msg != null) {
                _logger.Error(exc, msg);
            }
        }

        public void E(string tag, Exception exc)
        {
            if (tag != null) {
                _logger.Debug(exc, "");
            }
        }

// MARK: - Private Methods

        private static Logger.LogLevel FindMinimumLogLevel(ILogger logger)
        {
            if (logger.IsEnabled(LogEventLevel.Verbose)) {
                return Logger.LogLevel.Verbose;
            }
            if (logger.IsEnabled(LogEventLevel.Debug)) {
                return Logger.LogLevel.Debug;
            }
            if (logger.IsEnabled(LogEventLevel.Information)) {
                return Logger.LogLevel.Information;
            }
            if (logger.IsEnabled(LogEventLevel.Warning)) {
                return Logger.LogLevel.Warning;
            }
            return Logger.LogLevel.Error;
        }

// MARK: - Variables

        private readonly ILogger _logger;
    }
}