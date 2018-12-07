using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;

namespace Vostok.Logging.Hercules
{
    internal static partial class HerculesTagsBuilderExtensions
    {
        public static IHerculesTagsBuilder AddProperties(
            this IHerculesTagsBuilder builder,
            IReadOnlyDictionary<string, object> properties)
        {
            foreach (var keyValuePair in properties)
            {
                if (builder.TryAddPropertyOfHerculesType(keyValuePair.Key, keyValuePair.Value))
                    continue;
                builder.AddValue(keyValuePair.Key, ObjectValueFormatter.Format(keyValuePair.Value));
            }

            return builder;
        }

        public static IHerculesTagsBuilder AddExceptionData(
            this IHerculesTagsBuilder builder,
            Exception exception)
        {
            builder
                .AddValue("Message", exception.Message)
                .AddValue("Type", exception.GetType().FullName);
            
            var stackFrames = new StackTrace(exception).GetFrames();
            if (stackFrames != null)
                builder.AddVectorOfContainers(
                    "StackTrace",
                    stackFrames,
                    (tagsBuilder, frame) => tagsBuilder.AddStackFrameData(frame));
            
            if (exception.InnerException != null)
                builder.AddContainer(
                    "InnerException",
                    tagsBuilder => tagsBuilder.AddExceptionData(exception.InnerException));

            if (exception is AggregateException aggregateException)
                builder.AddVectorOfContainers(
                    "InnerExceptions",
                    aggregateException.InnerExceptions,
                    (tagsBuilder, e) => tagsBuilder.AddExceptionData(e));
            
            return builder;
        }

        private static IHerculesTagsBuilder AddStackFrameData(
            this IHerculesTagsBuilder builder,
            StackFrame frame)
        {
            var method = frame.GetMethod();
            if (method != null)
            {
                builder.AddValue("Function", method.Name);
                if (method.DeclaringType != null)
                    builder.AddValue("Class", method.DeclaringType.FullName);
            }

            var fileName = frame.GetFileName();
            if (fileName != null)
                builder.AddValue("File", fileName);
            
            var lineNumber = frame.GetFileLineNumber();
            if (lineNumber != -1)
                builder.AddValue("Line", lineNumber);
            
            var columnNumber = frame.GetFileColumnNumber();
            if (columnNumber != -1)
                builder.AddValue("Column", columnNumber);
            
            return builder;
        }
    }
}