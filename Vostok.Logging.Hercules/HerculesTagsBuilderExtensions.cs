using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules
{
    internal static class HerculesTagsBuilderExtensions
    {
        public static IHerculesTagsBuilder AddProperties(
            this IHerculesTagsBuilder builder,
            IReadOnlyDictionary<string, object> properties)
        {
            foreach (var keyValuePair in properties)
            {
                if (IsPositionalName(keyValuePair.Key))
                    continue;

                if (builder.TryAddObject(keyValuePair.Key, keyValuePair.Value))
                    continue;

                var value = keyValuePair.Value;
                var format = value is DateTime || value is DateTimeOffset ? "O" : null;

                builder.AddValue(keyValuePair.Key, ObjectValueFormatter.Format(value, format));
            }

            return builder;
        }

        public static IHerculesTagsBuilder AddExceptionData(
            this IHerculesTagsBuilder builder,
            Exception exception)
        {
            builder
                .AddValue(ExceptionTagNames.Message, exception.Message)
                .AddValue(ExceptionTagNames.Type, exception.GetType().FullName);

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
                builder.AddValue(StackFrameTagNames.Function, method.Name);
                if (method.DeclaringType != null)
                    builder.AddValue(StackFrameTagNames.Type, method.DeclaringType.FullName);
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