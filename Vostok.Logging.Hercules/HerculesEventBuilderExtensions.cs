using System;
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
            LogEvent @event, IFormatProvider formatProvider)
        {
            builder
                .SetTimestamp(@event.Timestamp)
                .AddValue(LogEventTagNames.TimeZone, @event.Timestamp.Offset.Ticks)
                .AddValue(LogEventTagNames.MessageTemplate, @event.MessageTemplate ?? string.Empty)
                .AddValue(LogEventTagNames.RenderedMessage, LogMessageFormatter.Format(@event, formatProvider));

            if (@event.Exception != null)
                builder.AddContainer(
                    LogEventTagNames.Exception,
                    tagsBuilder => tagsBuilder.AddExceptionData(@event.Exception));
                    
            if (@event.Properties != null)
                builder.AddContainer(
                    LogEventTagNames.Properties,
                    tagsBuilder => tagsBuilder.AddProperties(@event.Properties));


            return builder;
        }
    }
}