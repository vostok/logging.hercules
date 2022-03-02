using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Tests
{
    internal class HerculesTagsBuilderExtensions_Tests
    {
        private HerculesTagsBuilder builder;

        [SetUp]
        public void Setup()
        {
            builder = new HerculesEventBuilder();
        }

        [Test]
        public void Should_serialize_inner_exceptions()
        {
            var exception0 = new InvalidOperationException();
            var exception1 = new NullReferenceException();
            var exception2 = new ArgumentException();

            var aggregateException = new AggregateException(exception0, exception1, exception2);

            builder.AddExceptionData(aggregateException);

            var tags = builder.BuildTags();
            tags.ContainsKey(ExceptionTagNames.InnerExceptions).Should().BeTrue();
            var exceptions = tags[ExceptionTagNames.InnerExceptions].AsVector.AsContainerList;
            exceptions[0][ExceptionTagNames.Type].AsString.Should().Be(exception0.GetType().FullName);
            exceptions[1][ExceptionTagNames.Type].AsString.Should().Be(exception1.GetType().FullName);
            exceptions[2][ExceptionTagNames.Type].AsString.Should().Be(exception2.GetType().FullName);
        }

        [Test]
        public void Should_serialize_dates_in_ISO_format()
        {
            var dtoValue = DateTimeOffset.Now;
            var dtValue = DateTime.Now;

            var properties = new Dictionary<string, object>
            {
                ["DateTimeOffset"] = dtoValue,
                ["DateTime"] = dtValue
            };

            builder.AddProperties(new LogEvent(LogLevel.Info, DateTimeOffset.Now, null, properties, null) , null, null);

            var tags = builder.BuildTags();

            tags["DateTimeOffset"]?.AsString.Should().Be(dtoValue.ToString("O"));
            tags["DateTime"]?.AsString.Should().Be(dtValue.ToString("O"));
        }

        [Test]
        public void Should_filter_properties()
        {
            var properties = new Dictionary<string, object>
            {
                ["p1"] = "v1",
                ["p2"] = "v2"
            };

            builder.AddProperties(new LogEvent(LogLevel.Info, DateTimeOffset.Now, null, properties, null) , new[] {"p1"}, null);

            var tags = builder.BuildTags();

            tags.ContainsKey("p1").Should().BeFalse();
            tags["p2"]?.AsString.Should().Be("v2");
        }
    }
}