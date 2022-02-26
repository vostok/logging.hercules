using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;
using Vostok.Logging.Hercules.Configuration;
using Vostok.Logging.Hercules.Constants;

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

            log = new HerculesLog(new HerculesLogSettings(sink, stream));
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

            var exception = @event.Tags[LogEventTagNames.Exception]?.AsContainer;

            var topFrame = exception?[ExceptionTagNames.StackFrames]?.AsVector.AsContainerList[0];

            topFrame?[StackFrameTagNames.Function]
                ?.AsString
                .Should()
                .Be(nameof(GetExceptionWithStacktrace));

            topFrame?[StackFrameTagNames.Type]
                ?.AsString
                .Should()
                .Be($"Vostok.Logging.Hercules.Tests.{GetType().Name}");

            @event.Tags[LogEventTagNames.StackTrace]?.AsString.Should().Contain("Vostok.Logging.Hercules.Tests");
        }

        [Test]
        public void IsEnabledFor_should_respect_enabled_log_levels()
        {
            sink = Substitute.For<IHerculesSink>();

            log = new HerculesLog(new HerculesLogSettings(sink, "stream")
            {
                EnabledLogLevels = new[] {LogLevel.Debug, LogLevel.Warn, LogLevel.Fatal}
            });

            log.IsEnabledFor(LogLevel.Debug).Should().BeTrue();
            log.IsEnabledFor(LogLevel.Info).Should().BeFalse();
            log.IsEnabledFor(LogLevel.Warn).Should().BeTrue();
            log.IsEnabledFor(LogLevel.Error).Should().BeFalse();
            log.IsEnabledFor(LogLevel.Fatal).Should().BeTrue();
        }

        [Test]
        public void Log_should_respect_enabled_log_levels()
        {
            sink = Substitute.For<IHerculesSink>();

            log = new HerculesLog(new HerculesLogSettings(sink, "stream")
            {
                EnabledLogLevels = new[] {LogLevel.Debug, LogLevel.Warn, LogLevel.Fatal}
            });

            log.Debug("");
            sink.ReceivedCalls().Should().HaveCount(1);

            log.Info("");
            sink.ReceivedCalls().Should().HaveCount(1);

            log.Warn("");
            sink.ReceivedCalls().Should().HaveCount(2);

            log.Error("");
            sink.ReceivedCalls().Should().HaveCount(2);

            log.Fatal("");
            sink.ReceivedCalls().Should().HaveCount(3);
        }

        [Test]
        public void Should_save_SourceContextValue_as_vector_of_strings()
        {
            var builder = new HerculesEventBuilder();

            sink.Put(stream, Arg.Do<Action<IHerculesEventBuilder>>(x => x(builder)));

            log
                .ForContext("src1")
                .ForContext("src2")
                .Info("Hello!");

            var @event = builder.BuildEvent();

            var vector = @event.Tags[LogEventTagNames.Properties]?.AsContainer[WellKnownProperties.SourceContext]?.AsVector;

            vector?.AsStringList.Should().Equal("src1", "src2");
        }

        [Test]
        public void Should_save_OperationContextValue_as_vector_of_strings()
        {
            var builder = new HerculesEventBuilder();

            sink.Put(stream, Arg.Do<Action<IHerculesEventBuilder>>(x => x(builder)));

            log
                .WithProperty(WellKnownProperties.OperationContext, new OperationContextValue(new[] {"op1", "op2"}))
                .Info("Hello!");

            var @event = builder.BuildEvent();

            var vector = @event.Tags[LogEventTagNames.Properties]?.AsContainer[WellKnownProperties.OperationContext]?.AsVector;

            vector?.AsStringList.Should().Equal("op1", "op2");
        }
        
        [Test]
        public void Should_format_OperationContextValue_as_vector_of_strings()
        {
            var builder = new HerculesEventBuilder();

            sink.Put(stream, Arg.Do<Action<IHerculesEventBuilder>>(x => x(builder)));

            log
                .WithProperty(WellKnownProperties.OperationContext, new OperationContextValue(new[] {"op1", "{Name}", "op3"}))
                .Info("Hello {Name}!", "Vostok");

            var @event = builder.BuildEvent();

            var vector = @event.Tags[LogEventTagNames.Properties]?.AsContainer[WellKnownProperties.OperationContext]?.AsVector;

            vector?.AsStringList.Should().Equal("op1", "Vostok", "op3");
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