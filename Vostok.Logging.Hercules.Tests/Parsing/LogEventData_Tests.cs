using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;
using Vostok.Logging.Hercules.Parsing;

namespace Vostok.Logging.Hercules.Tests.Parsing
{
    public class LogEventData_Tests
    {
        [Test]
        public void Should_not_fail_on_empty_HerculesEvent()
        {
            var @event = new HerculesEvent(DateTimeOffset.Now, HerculesTags.Empty);

            var logEventData = new LogEventData(@event);

            logEventData.Exception.Should().BeNull();
            logEventData.Properties.Should().BeNull();
            logEventData.MessageTemplate.Should().BeNull();
            logEventData.StackTrace.Should().BeNull();
            logEventData.Message.Should().BeNull();
        }

        [Test]
        public void Should_parse_date()
        {
            var stackTrace = Guid.NewGuid().ToString();
            var messageTemplate = Guid.NewGuid().ToString();
            var renderedMessage = Guid.NewGuid().ToString();
            var propKey = "prop1";
            var propValue = 2;

            var eventBuilder = new HerculesEventBuilder();

            eventBuilder
                .SetTimestamp(DateTimeOffset.ParseExact("10/31/2022 6:36:14 AM +00:00", "MM/dd/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture.DateTimeFormat))
                .AddValue(LogEventTagNames.UtcOffset, 108000000000L)
                .AddValue(LogEventTagNames.MessageTemplate, messageTemplate)
                .AddValue(LogEventTagNames.Message, renderedMessage)
                .AddValue(LogEventTagNames.StackTrace, stackTrace)
                .AddContainer(LogEventTagNames.Properties, b => b.AddValue(propKey, propValue))
                .AddContainer(LogEventTagNames.Exception, delegate {});

            var @event = eventBuilder.BuildEvent();

            var logEventData = new LogEventData(@event);

            TestContext.Out.WriteLine(logEventData.Timestamp);
            
            logEventData.Timestamp.DateTime.Should().Be(DateTime.ParseExact("10/31/2022 9:36:14 AM", "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture.DateTimeFormat));
            logEventData.Timestamp.UtcDateTime.Should().Be(DateTime.ParseExact("10/31/2022 6:36:14 AM", "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture.DateTimeFormat));
        }

        [Test]
        public void Should_parse_date_without_offset()
        {
            var stackTrace = Guid.NewGuid().ToString();
            var messageTemplate = Guid.NewGuid().ToString();
            var renderedMessage = Guid.NewGuid().ToString();
            var propKey = "prop1";
            var propValue = 2;

            var eventBuilder = new HerculesEventBuilder();

            eventBuilder
                .SetTimestamp(DateTimeOffset.ParseExact("10/31/2022 6:36:14 AM +00:00", "MM/dd/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture.DateTimeFormat))
                .AddValue(LogEventTagNames.MessageTemplate, messageTemplate)
                .AddValue(LogEventTagNames.Message, renderedMessage)
                .AddValue(LogEventTagNames.StackTrace, stackTrace)
                .AddContainer(LogEventTagNames.Properties, b => b.AddValue(propKey, propValue))
                .AddContainer(LogEventTagNames.Exception, delegate {});

            var @event = eventBuilder.BuildEvent();

            var logEventData = new LogEventData(@event);

            TestContext.Out.WriteLine(logEventData.Timestamp);
            
            logEventData.Timestamp.DateTime.Should().Be(DateTime.ParseExact("10/31/2022 6:36:14 AM", "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture.DateTimeFormat));
            logEventData.Timestamp.UtcDateTime.Should().Be(DateTime.ParseExact("10/31/2022 6:36:14 AM", "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture.DateTimeFormat));
        }
        
        [Test]
        public void Should_extract_values()
        {
            var offset = 10.Hours();
            var timestamp = DateTimeOffset.Now.ToOffset(offset);
            var stackTrace = Guid.NewGuid().ToString();
            var messageTemplate = Guid.NewGuid().ToString();
            var renderedMessage = Guid.NewGuid().ToString();
            var propKey = "prop1";
            var propValue = 2;

            var eventBuilder = new HerculesEventBuilder();

            eventBuilder
                .SetTimestamp(timestamp)
                .AddValue(LogEventTagNames.MessageTemplate, messageTemplate)
                .AddValue(LogEventTagNames.Message, renderedMessage)
                .AddValue(LogEventTagNames.StackTrace, stackTrace)
                .AddContainer(LogEventTagNames.Properties, b => b.AddValue(propKey, propValue))
                .AddContainer(LogEventTagNames.Exception, delegate {});

            var @event = eventBuilder.BuildEvent();

            var logEventData = new LogEventData(@event);

            logEventData.Timestamp.Should().Be(timestamp);
            logEventData.MessageTemplate.Should().Be(messageTemplate);
            logEventData.Message.Should().Be(renderedMessage);
            logEventData.StackTrace.Should().Be(stackTrace);
            logEventData.Properties.Should().NotBeNull();
            logEventData.Properties[propKey].AsInt.Should().Be(propValue);
            logEventData.Exception.Should().NotBeNull();
        }
    }
}