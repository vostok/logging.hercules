using System;
using System.Runtime.InteropServices;
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
            logEventData.RenderedMessage.Should().BeNull();
        }

        [Test]
        public void Should_extract_values()
        {
            var offset = 10.Hours();
            var timestamp = DateTimeOffset.Now.ToOffset(offset);
            var messageTemplate = Guid.NewGuid().ToString();
            var renderedMessage = Guid.NewGuid().ToString();
            var propKey = "prop1";
            var propValue = 2;
            
            var eventBuilder = new HerculesEventBuilder();
            
            eventBuilder
                .SetTimestamp(timestamp)
                .AddValue(LogEventTagNames.MessageTemplate, messageTemplate)
                .AddValue(LogEventTagNames.RenderedMessage, renderedMessage)
                .AddContainer(LogEventTagNames.Properties, b => b.AddValue(propKey, propValue))
                .AddContainer(LogEventTagNames.Exception, delegate { });

            var @event = eventBuilder.BuildEvent();

            var logEventData = new LogEventData(@event);

            logEventData.Timestamp.Should().Be(timestamp);
            logEventData.MessageTemplate.Should().Be(messageTemplate);
            logEventData.RenderedMessage.Should().Be(renderedMessage);
            logEventData.Properties.Should().NotBeNull();
            logEventData.Properties[propKey].AsInt.Should().Be(propValue);
            logEventData.Exception.Should().NotBeNull();
        }
    }
}