using System;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

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
                .AddValue("TimeZone", @event.Timestamp.Offset.Ticks)
                .AddValue("MessageTemplate", @event.MessageTemplate ?? string.Empty)
                .AddValue("RenderedMessage", LogMessageFormatter.Format(@event, formatProvider));

            if (@event.Exception != null)
                builder.AddContainer(
                    "Exception",
                    tagsBuilder => tagsBuilder.AddExceptionData(@event.Exception));
                    
            if (@event.Properties != null)
                builder.AddContainer(
                    "Properties",
                    tagsBuilder => tagsBuilder.AddProperties(@event.Properties));


            return builder;
        }
    }
}