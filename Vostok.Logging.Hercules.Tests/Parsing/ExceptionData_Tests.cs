using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;
using Vostok.Logging.Hercules.Parsing;

namespace Vostok.Logging.Hercules.Tests.Parsing
{
    public class ExceptionData_Tests
    {
        [Test]
        public void Should_not_fail_on_empty_tags()
        {
            var logEventData = ExceptionData.FromTags(HerculesTags.Empty);

            logEventData.Type.Should().BeNull();
            logEventData.Message.Should().BeNull();
            logEventData.InnerExceptions.Should().BeNull();
        }

        [Test]
        public void Should_extract_values()
        {
            var type = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            var tagsBuilder = new HerculesTagsBuilder();

            tagsBuilder
                .AddValue(ExceptionTagNames.Type, type)
                .AddValue(ExceptionTagNames.Message, message)
                .AddVectorOfContainers(ExceptionTagNames.StackFrames, new Action<IHerculesTagsBuilder>[0])
                .AddVectorOfContainers(ExceptionTagNames.InnerExceptions, new Action<IHerculesTagsBuilder>[0]);

            var tags = tagsBuilder.BuildTags();

            var exceptionData = ExceptionData.FromTags(tags);

            exceptionData.Type.Should().Be(type);
            exceptionData.Message.Should().Be(message);
            exceptionData.StackFrames.Should().NotBeNull();
            exceptionData.InnerExceptions.Should().NotBeNull();
        }
    }
}