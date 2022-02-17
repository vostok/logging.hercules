using System;
using System.Collections.Generic;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules
{
    internal static class HerculesEventBuilderExtensions
    {
        public static IHerculesTagsBuilder AddLogEventData(
            this IHerculesEventBuilder builder,
            LogEvent @event,
            IReadOnlyCollection<string> filteredProperties,
            IFormatProvider formatProvider)
        {
            builder
                .SetTimestamp(@event.Timestamp)
                .AddValue(LogEventTagNames.UtcOffset, @event.Timestamp.Offset.Ticks)
                .AddValue(LogEventTagNames.Level, @event.Level.ToString())
                .AddValue(LogEventTagNames.Message, LogMessageFormatter.Format(@event, formatProvider));

            if (@event.MessageTemplate != null)
                builder.AddValue(LogEventTagNames.MessageTemplate, @event.MessageTemplate);

            if (@event.Exception != null)
            {
                builder.AddContainer(
                    LogEventTagNames.Exception,
                    tagsBuilder => tagsBuilder.AddExceptionData(@event.Exception));

                if (@event.Exception.StackTrace != null)
                    builder.AddValue(LogEventTagNames.StackTrace, @event.Exception.ToString());
            }

            if (@event.Properties != null)
                builder.AddContainer(
                    LogEventTagNames.Properties,
                    tagsBuilder => tagsBuilder.AddProperties(@event.Properties, filteredProperties, formatProvider));

            return builder;
        }
    }
}