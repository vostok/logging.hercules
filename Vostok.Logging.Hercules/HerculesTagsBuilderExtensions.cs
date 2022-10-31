using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;
using Vostok.Logging.Formatting;
using Vostok.Logging.Hercules.Constants;
using Vostok.Logging.Hercules.Helpers;

namespace Vostok.Logging.Hercules
{
    internal static class HerculesTagsBuilderExtensions
    {
        private const int MaximumRecursionDepth = 10;
        
        public static IHerculesTagsBuilder AddProperties(
            this IHerculesTagsBuilder builder,
            LogEvent @event,
            IReadOnlyCollection<string> filteredProperties,
            IFormatProvider formatProvider)
        {
            foreach (var keyValuePair in @event.Properties!)
            {
                var key = keyValuePair.Key;
                if (IsPositionalName(key))
                    continue;
                if (filteredProperties?.Contains(key) == true)
                    continue;

                var value = keyValuePair.Value;

                if (key == WellKnownProperties.OperationContext && value is OperationContextValue operationContextValue)
                    value = operationContextValue.Select(t => OperationContextValueFormatter.Format(@event, t, null, formatProvider)).ToArray(); 

                builder.AddProperty(key, value, formatProvider, 0);
            }

            return builder;
        }

        public static IHerculesTagsBuilder AddExceptionData(
            this IHerculesTagsBuilder builder,
            Exception exception)
        {
            builder
                .AddValue(ExceptionTagNames.Message, exception.Message)
                .AddValue(ExceptionTagNames.Type, ExceptionsNormalizer.Normalize(exception.GetType().FullName));

            var stackFrames = new StackTrace(exception, true).GetFrames();
            if (stackFrames != null)
                builder.AddVectorOfContainers(
                    ExceptionTagNames.StackFrames,
                    stackFrames,
                    (tagsBuilder, frame) => tagsBuilder.AddStackFrameData(frame));

            var innerExceptions = new List<Exception>();

            if (exception is AggregateException aggregateException)
                innerExceptions.AddRange(aggregateException.InnerExceptions);
            else if (exception.InnerException != null)
                innerExceptions.Add(exception.InnerException);

            if (innerExceptions.Count > 0)
                builder.AddVectorOfContainers(
                    ExceptionTagNames.InnerExceptions,
                    innerExceptions,
                    (tagsBuilder, e) => tagsBuilder.AddExceptionData(e));

            return builder;
        }

        private static void AddStackFrameData(this IHerculesTagsBuilder builder, StackFrame frame)
        {
            var method = frame.GetMethod();
            if (method != null)
            {
                builder.AddValue(StackFrameTagNames.Function, ExceptionsNormalizer.Normalize(method.Name));
                if (method.DeclaringType != null)
                    builder.AddValue(StackFrameTagNames.Type, ExceptionsNormalizer.Normalize(method.DeclaringType.FullName));
            }

            var fileName = frame.GetFileName();
            if (!string.IsNullOrEmpty(fileName))
                builder.AddValue(StackFrameTagNames.File, fileName);

            var lineNumber = frame.GetFileLineNumber();
            if (lineNumber > 0)
                builder.AddValue(StackFrameTagNames.Line, lineNumber);

            var columnNumber = frame.GetFileColumnNumber();
            if (columnNumber > 0)
                builder.AddValue(StackFrameTagNames.Column, columnNumber);
        }
        
        private static void AddProperty(
            this IHerculesTagsBuilder builder,
            string key,
            object value,
            IFormatProvider formatProvider,
            int depth)
        {
            Type valueType;

            if (builder.TryAddObject(key, value) || depth > MaximumRecursionDepth)
            {
                // return;
            }
            else if (value is IFormattable formattable)
            {
                var format = value is DateTime or DateTimeOffset ? "O" : null;
                builder.AddValue(key, formattable.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            }
            else if (false) // todo (kungurtsev, 20.02.2022): check should not deconstruct
            {
                builder.AddValue(key, ObjectValueFormatter.Format(value, null, formatProvider));
            }
            else if (DictionaryInspector.IsSimpleDictionary(valueType = value.GetType()))
            {
                builder.AddContainer(key,
                    tagsBuilder =>
                    {
                        foreach (var (pKey, pValue) in DictionaryInspector.EnumerateSimpleDictionary(value))
                            tagsBuilder.AddProperty(pKey, pValue, formatProvider, depth + 1);
                    });
            }
            else if (value is IEnumerable enumerable)
            {
                builder.AddVectorOfContainers(key,
                    enumerable.Cast<object>().ToList(),
                    (tagsBuilder, element) =>
                    {
                        if (element != null && ObjectPropertiesExtractor.HasProperties(element.GetType()))
                        {
                            foreach (var (eKey, eValue) in ObjectPropertiesExtractor.ExtractProperties(element))
                                tagsBuilder.AddProperty(eKey, eValue, formatProvider, depth + 1);
                        }
                        else
                        {
                            tagsBuilder.AddProperty("key", element, formatProvider, depth + 1);
                        }
                    });
            }
            else if (ObjectPropertiesExtractor.HasProperties(valueType)) // todo (kungurtsev, 20.02.2022): or check here?
            {
                builder.AddContainer(key,
                    tagsBuilder =>
                    {
                        foreach (var (pKey, pValue) in ObjectPropertiesExtractor.ExtractProperties(value))
                            tagsBuilder.AddProperty(pKey, pValue, formatProvider, depth + 1);
                    });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPositionalName(string propertyName)
        {
            foreach (var character in propertyName)
            {
                if (character < '0')
                    return false;

                if (character > '9')
                    return false;
            }

            return true;
        }
    }
}