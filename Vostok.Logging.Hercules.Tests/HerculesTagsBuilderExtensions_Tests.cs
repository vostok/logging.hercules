using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
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
    }
}