using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Hercules.Tests
{
    internal class HerculesLog_Tests
    {
        [TestCase(1)]
        [TestCase(3)]
        public void Should_put_events_to_HerculesSink(int eventsCount)
        {
            var stream = "test_stream_for_logs";
            var sink = Substitute.For<IHerculesSink>();
            
            var log = new HerculesLog(new HerculesLogSettings
            {
                HerculesSink = sink,
                Stream = stream
            });

            for (var i = 0; i < eventsCount; i++)
                log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, Guid.NewGuid().ToString()));

            sink.Received(eventsCount).Put(stream, Arg.Any<Action<IHerculesEventBuilder>>());
        }
    }
}