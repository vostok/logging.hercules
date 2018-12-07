using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Hercules.Tests
{
    internal class HerculesLog_Tests
    {
        private string stream;
        private IHerculesSink sink;
        private HerculesLog log;

        [SetUp]
        public void Setup()
        {
            stream = "test_stream_for_logs";
            sink = Substitute.For<IHerculesSink>();
            
            log = new HerculesLog(new HerculesLogSettings
            {
                HerculesSink = sink,
                Stream = stream
            });

        }
        
        [TestCase(1)]
        [TestCase(3)]
        public void Should_put_events_to_HerculesSink(int eventsCount)
        {
            for (var i = 0; i < eventsCount; i++)
                log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, Guid.NewGuid().ToString()));

            sink.Received(eventsCount).Put(stream, Arg.Any<Action<IHerculesEventBuilder>>());
        }
        
        [Test]
        public void Should_put_event_with_exception()
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, Guid.NewGuid().ToString(), GetExceptionWithStacktrace()));

            sink.Received(1).Put(stream, Arg.Any<Action<IHerculesEventBuilder>>());
        }
        
        [Test]
        public void Should_add_exception_data_to_event()
        {
            var builder = new HerculesEventBuilder();
            
            sink.Put(stream, Arg.Do<Action<IHerculesEventBuilder>>(x => x(builder)));

            var e = GetExceptionWithStacktrace();

            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, Guid.NewGuid().ToString(), e));

            var @event = builder.BuildEvent();

            var exception = @event.Tags["Exception"].AsContainer;

            var topFrame = exception["StackTrace"].AsVector.AsContainerArray[0];
            
            topFrame["Function"]
                .AsString
                .Should()
                .Be(nameof(GetExceptionWithStacktrace));
            
            topFrame["Class"]
                .AsString
                .Should()
                .Be("Vostok.Logging.Hercules.Tests.HerculesLog_Tests");
        }

        private static Exception GetExceptionWithStacktrace()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}