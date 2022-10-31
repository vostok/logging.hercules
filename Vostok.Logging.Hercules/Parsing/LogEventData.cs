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
        private volatile ExceptionData exception;
        private volatile IDictionary<string, HerculesValue> properties;
        private LogLevel? level;

        /// <param name="event"><see cref="HerculesEvent"/> that represents serialized <see cref="LogEvent"/>.</param>
        public LogEventData(HerculesEvent @event)
        {
            this.@event = @event;
            
            var timestampOffset = new TimeSpan(@event.Tags[LogEventTagNames.UtcOffset]?.AsLong ?? default);
            var timestamp = DateTime.SpecifyKind(@event.Timestamp.UtcDateTime + timestampOffset, DateTimeKind.Unspecified);
            Timestamp = new DateTimeOffset(timestamp, timestampOffset);
        }

        /// <summary>
        /// <para>The timestamp of original <see cref="LogEvent"/>.</para>
        /// <para>For more information see <see cref="LogEvent.Timestamp"/>.</para>
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// <para>The level of original <see cref="LogEvent"/>.</para>
        /// <para>For more information see <see cref="LogEvent.Level"/>.</para>
        /// </summary>
        public LogLevel Level => level ?? (level = ParseLogLevel(@event.Tags[LogEventTagNames.Level]?.AsString)).Value;

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
        public string Message => @event.Tags[LogEventTagNames.Message]?.AsString;

        /// <summary>
        /// <para>The error associated with this log event.</para>
        /// <para>See: <see cref="LogEvent.Exception"/>.</para>
        /// </summary>
        [CanBeNull]
        public ExceptionData Exception => exception ?? (exception = ExceptionData.FromTags(@event.Tags[LogEventTagNames.Exception]?.AsContainer));

        /// <summary>
        /// The string representation of <see cref="Exception"/> stacktrace.
        /// </summary>
        [CanBeNull]
        public string StackTrace => @event.Tags[LogEventTagNames.StackTrace]?.AsString;

        /// <summary>
        /// <para>Contains various user-defined properties of the event.</para>
        /// <para>For more information see <see cref="LogEvent.Properties"/>.</para>
        /// </summary>
        [CanBeNull]
        public IDictionary<string, HerculesValue> Properties => properties ?? (properties = ReadDictionary(LogEventTagNames.Properties));

        private static LogLevel ParseLogLevel(string value)
        {
            switch (value)
            {
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value);
            }
        }

        private IDictionary<string, HerculesValue> ReadDictionary(string tagName) => @event.Tags[tagName]
            ?.AsContainer
            ?.ToDictionary(
                x => x.Key,
                x => x.Value);
    }
}