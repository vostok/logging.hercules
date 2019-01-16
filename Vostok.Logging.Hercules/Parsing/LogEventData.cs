using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Hercules.Client.Abstractions.Values;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    /// <summary>
    /// A class that provides deserialization functionality for <see cref="LogEvent"/> data serialized to <see cref="HerculesEvent"/>.
    /// </summary>
    [PublicAPI]
    public class LogEventData
    {
        private readonly HerculesEvent @event;
        private ExceptionData exceptionData;

        /// <param name="event"><see cref="HerculesEvent"/> that represents serialized <see cref="LogEvent"/>.</param>
        public LogEventData(HerculesEvent @event)
        {
            this.@event = @event;
            Timestamp = new DateTimeOffset(
                DateTime.SpecifyKind(@event.Timestamp.UtcDateTime, DateTimeKind.Unspecified),
                new TimeSpan(@event.Tags[LogEventTagNames.UtcOffset]?.AsLong ?? default));
        }

        /// <summary>
        /// <para>The timestamp of original <see cref="LogEvent"/>.</para>
        /// <para>For more information see <see cref="LogEvent.Timestamp"/>.</para>
        /// </summary>
        public DateTimeOffset Timestamp { get; }
        
        /// <summary>
        /// <para>The template of the log message containing placeholders to be filled with values from <see cref="Properties"/>.</para>
        /// <para>Can be null for events containing only <see cref="Exception"/>.</para>
        /// <para>For more information see <see cref="LogEvent.MessageTemplate"/>.</para>
        /// </summary>
        [CanBeNull]
        public string MessageTemplate => @event.Tags[LogEventTagNames.MessageTemplate]?.AsString;
        
        /// <summary>
        /// <para>The text representation of <see cref="LogEvent"/> message based on <see cref="MessageTemplate"/> and <see cref="Properties"/>.</para>
        /// </summary>
        [CanBeNull]
        public string RenderedMessage => @event.Tags[LogEventTagNames.RenderedMessage]?.AsString;
        
        /// <summary>
        /// <para>The error associated with this log event.</para>
        /// <para>See: <see cref="LogEvent.Exception"/>.</para>
        /// </summary>
        [CanBeNull]
        public ExceptionData Exception => ExceptionData.FromTags(@event.Tags[LogEventTagNames.Exception]?.AsContainer);

        /// <summary>
        /// <para>Contains various user-defined properties of the event.</para>
        /// <para>For more information see <see cref="LogEvent.Properties"/>.</para>
        /// </summary>
        [CanBeNull]
        public IDictionary<string, HerculesValue> Properties => ReadDictionary(LogEventTagNames.Properties);

        private IDictionary<string, HerculesValue> ReadDictionary(string tagName) => @event.Tags[tagName]
            ?.AsContainer
            ?.ToDictionary(
                x => x.Key,
                x => x.Value);
    }
}