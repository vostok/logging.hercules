using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                if (builder.TryAddObject(keyValuePair.Key, keyValuePair.Value))
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
                .AddValue(ExceptionTagNames.Message, exception.Message)
                .AddValue(ExceptionTagNames.Type, exception.GetType().FullName);
            
            var stackFrames = new StackTrace(exception).GetFrames();
            if (stackFrames != null)
                builder.AddVectorOfContainers(
                    ExceptionTagNames.StackTrace,
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

        private static IHerculesTagsBuilder AddStackFrameData(
            this IHerculesTagsBuilder builder,
            StackFrame frame)
        {
            var method = frame.GetMethod();
            if (method != null)
            {
                builder.AddValue(StackFrameTagNames.Function, method.Name);
                if (method.DeclaringType != null)
                    builder.AddValue(StackFrameTagNames.Type, method.DeclaringType.FullName);
            }

            var fileName = frame.GetFileName();
            if (fileName != null)
                builder.AddValue(StackFrameTagNames.File, fileName);
            
            var lineNumber = frame.GetFileLineNumber();
            if (lineNumber != -1)
                builder.AddValue(StackFrameTagNames.Line, lineNumber);
            
            var columnNumber = frame.GetFileColumnNumber();
            if (columnNumber != -1)
                builder.AddValue(StackFrameTagNames.Column, columnNumber);
            
            return builder;
        }
    }
}