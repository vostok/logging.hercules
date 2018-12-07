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
                .AddValue(LogEventFields.TimeZone, @event.Timestamp.Offset.Ticks)
                .AddValue(LogEventFields.MessageTemplate, @event.MessageTemplate ?? string.Empty)
                .AddValue(LogEventFields.RenderedMessage, LogMessageFormatter.Format(@event, formatProvider));

            if (@event.Exception != null)
                builder.AddContainer(
                    LogEventFields.Exception,
                    tagsBuilder => tagsBuilder.AddExceptionData(@event.Exception));
                    
            if (@event.Properties != null)
                builder.AddContainer(
                    LogEventFields.Properties,
                    tagsBuilder => tagsBuilder.AddProperties(@event.Properties));


            return builder;
        }
    }
}