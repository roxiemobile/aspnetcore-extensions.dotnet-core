using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

// File logging on ASP.NET Core
// @link http://gunnarpeipman.com/2017/01/aspnet-core-file-logging/

// Simple .NET logging with fully-structured events
// @link https://github.com/serilog/serilog

// A console sink for Serilog that pretty-prints embedded properties
// @link https://github.com/serilog/serilog-sinks-literate

// Write log events to a set of rolling log files
// @link https://github.com/serilog/serilog-sinks-rollingfile

// An asynchronous wrapper for Serilog sinks that logs on a background thread
// @link https://github.com/serilog/serilog-sinks-async

namespace RoxieMobile.AspNetCore.Logging.Serilog
{
    /// <summary>
    /// Extends <see cref="LoggerSinkConfiguration"/> with methods for configuring
    /// asynchronous logging to rolling file.
    /// </summary>
    public static class RollingFileAsyncLoggerConfigurationExtensions
    {
// MARK: - Methods

        /// <summary>
        /// Asynchronously write log events to a series of files. Each file will be named according to
        /// the date of the first log entry written to it. Only simple date-based rolling is
        /// currently supported.
        /// </summary>
        /// <param name="loggerSinkConfiguration">Logger sink configuration.</param>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which any single log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not.
        /// The default is false.</param>
        /// <param name="shared">Allow the log files to be shared by multiple processes. The default is false.</param>
        /// <param name="flushToDiskInterval">If provided, a full disk flush will be performed periodically
        /// at the specified interval.</param>
        /// <param name="asyncBufferSize">The size of the concurrent queue used to feed the background worker thread. If
        /// the thread is unable to process events quickly enough and the queue is filled, subsequent events will be
        /// dropped until room is made in the queue.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 encoding without a byte-order mark.</remarks>
        public static LoggerConfiguration RollingFileAsync(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool buffered = false,
            bool shared = false,
            TimeSpan? flushToDiskInterval = null,
            int asyncBufferSize = 10000)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return RollingFileAsync(
                loggerSinkConfiguration, formatter, pathFormat, restrictedToMinimumLevel,
                fileSizeLimitBytes, retainedFileCountLimit, levelSwitch, buffered,
                shared, flushToDiskInterval, asyncBufferSize);
        }

        /// <summary>
        /// Write log events to a series of files. Each file will be named according to
        /// the date of the first log entry written to it. Only simple date-based rolling is
        /// currently supported.
        /// </summary>
        /// <param name="loggerSinkConfiguration">Logger sink configuration.</param>
        /// <param name="formatter">Formatter to control how events are rendered into the file. To control
        /// plain text formatting, use the overload that accepts an output template instead.</param>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which any single log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not.
        /// The default is false.</param>
        /// <param name="shared">Allow the log files to be shared by multiple processes. The default is false.</param>
        /// <param name="flushToDiskInterval">If provided, a full disk flush will be performed periodically at the specified interval.</param>
        /// <param name="asyncBufferSize">The size of the concurrent queue used to feed the background worker thread. If
        /// the thread is unable to process events quickly enough and the queue is filled, subsequent events will be
        /// dropped until room is made in the queue.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 encoding without a byte-order mark.</remarks>
        public static LoggerConfiguration RollingFileAsync(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            ITextFormatter formatter,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool buffered = false,
            bool shared = false,
            TimeSpan? flushToDiskInterval = null,
            int asyncBufferSize = 10000)
        {
            return loggerSinkConfiguration.Async(
                lsc => {
                    lsc.RollingFile(
                        formatter, pathFormat, restrictedToMinimumLevel, fileSizeLimitBytes,
                        retainedFileCountLimit, levelSwitch, buffered, shared, flushToDiskInterval);
                },
                asyncBufferSize);
        }

// MARK: - Constants

        private const string DefaultOutputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.ffffff K}] {RequestId,13} [{Level:u3}]" +
            " {Message} ({EventId:x8}){NewLine}{Exception}";

        private const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024;

        private const int DefaultRetainedFileCountLimit = 31; // A long month of logs
    }
}