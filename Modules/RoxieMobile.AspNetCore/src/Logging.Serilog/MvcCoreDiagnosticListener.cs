using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DiagnosticAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoxieMobile.CSharpCommons.Extensions;
using RoxieMobile.CSharpCommons.Logging;
using Serilog;

// Logging using DiagnosticSource in ASP.NET Core
// @link https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/

namespace RoxieMobile.AspNetCore.Logging.Serilog
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public class MvcCoreDiagnosticListener
    {
// MARK: - Methods

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeActionMethod")]
        public virtual void OnBeforeActionMethod(
            ActionContext actionContext,
            IReadOnlyDictionary<string, object?>? actionArguments,
            object controller)
        {
            if (Logger.IsNotLoggable(Logger.LogLevel.Information)) {
                return;
            }

            var actionName = actionContext.ActionDescriptor.DisplayName;
            Logger.I(TAG, $"Executing action method {actionName}");

            if (actionArguments.IsNotEmpty() && Logger.IsLoggable(Logger.LogLevel.Debug)) {

                var stringWriter = new StringWriter();
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.TextWriter(stringWriter, outputTemplate: "{Message:l}")
                    .CreateLogger();

                var convertedArguments = new List<string>(actionArguments.Count);
                PrepareArguments(actionArguments, actionContext.ActionDescriptor)?
                    .ForEach(pair => {

                        var value = pair.Value;
                        string messageTemplate;

                        if (value == null || BuiltInScalarTypes.Contains(value.GetType())) {
                            messageTemplate = "{Key}: {Value}";
                        }
                        else if (value is JsonElement jsonElement) {
                            messageTemplate = "{Key}: {Value:l}";
                            value = jsonElement.ToString();
                        }
                        else if (value is JToken jsonToken) {
                            messageTemplate = "{Key}: {Value:l}";
                            value = jsonToken.ToString(Formatting.None);
                        }
                        else {
                            messageTemplate = "{Key}: {@Value:l}";
                        }

                        logger.Information(messageTemplate, pair.Key, value);
                        convertedArguments.Add(stringWriter.ToString());

                        // Clear all content in XmlTextWriter and StringWriter
                        // @link http://stackoverflow.com/a/13706647

                        stringWriter.GetStringBuilder().Clear();
                    });

                if (convertedArguments.IsNotEmpty()) {
                    var joinedArguments = string.Join(", ", convertedArguments);
                    Logger.D(TAG, $"  Arguments: {{ {joinedArguments} }}");
                }
            }

            var validationState = actionContext.ModelState.ValidationState;
            Logger.I(TAG, $"  Validation state: {validationState}");
        }

// MARK: - Private Methods

        private static List<KeyValuePair<string, object?>>? PrepareArguments(
            IReadOnlyDictionary<string, object?> actionParameters,
            ActionDescriptor actionDescriptor)
        {
            var parameterDescriptors = actionDescriptor.Parameters;

            var count = parameterDescriptors.Count;
            if (count == 0) return null;

            var arguments = new List<KeyValuePair<string, object?>>();
            for (var index = 0; index < count; index++) {

                var parameterDescriptor = parameterDescriptors[index] as ControllerParameterDescriptor;
                if (parameterDescriptor == null) continue;

                var bindingSource = parameterDescriptor.BindingInfo?.BindingSource;
                if (bindingSource?.Id == "Services") continue;

                var parameterInfo = parameterDescriptor.ParameterInfo;
                var parameterName = parameterInfo.Name;
                if (parameterName.IsNotEmpty()) {

                    if (!actionParameters.TryGetValue(parameterName, out var value)) {
                        value = GetDefaultValueForParameter(parameterInfo);
                    }

                    arguments.Add(new KeyValuePair<string, object?>(parameterName, value));
                }
            }
            return arguments;
        }

        private static object? GetDefaultValueForParameter(
            ParameterInfo parameterInfo)
        {
            object? defaultValue;

            if (parameterInfo.HasDefaultValue) {
                defaultValue = parameterInfo.DefaultValue;
            }
            else {
                var defaultValueAttribute = parameterInfo
                    .GetCustomAttribute<DefaultValueAttribute>(inherit: false);

                if (defaultValueAttribute?.Value == null) {
                    defaultValue = parameterInfo.ParameterType.GetTypeInfo().IsValueType
                        ? Activator.CreateInstance(parameterInfo.ParameterType)
                        : null;
                }
                else {
                    defaultValue = defaultValueAttribute.Value;
                }
            }
            return defaultValue;
        }

// MARK: - Constants

        private static readonly Type TAG = typeof(MvcCoreDiagnosticListener);

        private static readonly HashSet<Type> BuiltInScalarTypes = new HashSet<Type> {
            // @formatter:off
            typeof(bool),
            typeof(char),
            typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
            typeof(string),
            typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan),
            typeof(Guid), typeof(Uri)
            // @formatter:on
        };
    }
}
