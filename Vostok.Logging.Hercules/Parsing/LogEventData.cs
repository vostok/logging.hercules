using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    [PublicAPI]
    public class LogEventData
    {
        private readonly HerculesEvent @event;
        private ExceptionData exceptionData;

        public LogEventData(HerculesEvent @event)
        {
            this.@event = @event;
            Timestamp = new DateTimeOffset(
                @event.Timestamp.UtcDateTime, 
                new TimeSpan(@event.Tags[LogEventTagNames.UtcOffset]?.AsLong ?? default));
        }

        public DateTimeOffset Timestamp { get; }
        public string MessageTemplate => @event.Tags[LogEventTagNames.MessageTemplate]?.AsString;
        public string RenderedMessage => @event.Tags[LogEventTagNames.RenderedMessage]?.AsString;
        public ExceptionData Exception => ExceptionData.FromTags(@event.Tags[LogEventTagNames.Exception]?.AsContainer);
        public IDictionary<string, object> Properties { get; }
        public IDictionary<string, object> AdditionalFields { get; }
    }
}