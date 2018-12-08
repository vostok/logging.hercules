using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Tests
{
    internal class HerculesEventBuilderExtensions_Tests
    {
        private DateTimeOffset timestamp;
        private HerculesEventBuilder builder;
        private TimeSpan utcOffset;

        [SetUp]
        public void Setup()
        {
            utcOffset = 3.Hours();
            
            timestamp = new DateTimeOffset(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), utcOffset);

            builder = new HerculesEventBuilder();
        }
        
        [Test]
        public void Should_serialize_timestamp_from_event()
        {
            var @event = new LogEvent(LogLevel.Info, timestamp, null);

            builder.AddLogEventData(@event, null);

            builder.BuildEvent().Timestamp.Should().Be(timestamp);
        }
        
        [Test]
        public void Should_serialize_utcOffset_from_event()
        {
            var @event = new LogEvent(LogLevel.Info, timestamp, null);

            builder.AddLogEventData(@event, null);

            builder.BuildEvent().Tags[LogEventTagNames.TimeZone].AsLong.Should().Be(utcOffset.Ticks);
        }
    }
}