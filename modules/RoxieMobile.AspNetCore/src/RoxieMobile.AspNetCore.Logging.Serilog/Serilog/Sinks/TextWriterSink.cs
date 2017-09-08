using System;
using System.IO;
using RoxieMobile.CSharpCommons.Diagnostics;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace RoxieMobile.AspNetCore.Logging.Serilog.Sinks
{
    internal class TextWriterSink : ILogEventSink
    {
// MARK: - Construction

        public TextWriterSink(TextWriter textWriter, ITextFormatter textFormatter)
        {
            Guard.NotNull(textWriter, Funcs.Null(nameof(textWriter)));
            Guard.NotNull(textFormatter, Funcs.Null(nameof(textFormatter)));

            // Init instance valiables
            _textWriter = textWriter;
            _textFormatter = textFormatter;
        }

// MARK: - Methods

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) {
                throw new ArgumentNullException(nameof(logEvent));
            }

            lock (_syncLock) {
                _textFormatter.Format(logEvent, _textWriter);
                _textWriter.Flush();
            }
        }

// MARK: - Variables

        private readonly TextWriter _textWriter;

        private readonly ITextFormatter _textFormatter;

        private readonly object _syncLock = new object();
    }
}