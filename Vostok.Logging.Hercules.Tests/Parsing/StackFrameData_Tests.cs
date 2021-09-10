using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;
using Vostok.Logging.Hercules.Parsing;

namespace Vostok.Logging.Hercules.Tests.Parsing
{
    public class StackFrameData_Tests
    {
        [Test]
        public void Should_not_fail_on_empty_tags()
        {
            var stackFrameData = new StackFrameData(HerculesTags.Empty);

            stackFrameData.Type.Should().BeNull();
            stackFrameData.Function.Should().BeNull();
            stackFrameData.File.Should().BeNull();
            stackFrameData.Line.Should().BeNull();
            stackFrameData.Column.Should().BeNull();
        }

        [Test]
        public void Should_extract_values()
        {
            var type = Guid.NewGuid().ToString();
            var function = Guid.NewGuid().ToString();
            var file = Guid.NewGuid().ToString();
            var line = 25;
            var column = 80;

            var tagsBuilder = new HerculesTagsBuilder();

            tagsBuilder
                .AddValue(StackFrameTagNames.Type, type)
                .AddValue(StackFrameTagNames.Function, function)
                .AddValue(StackFrameTagNames.File, file)
                .AddValue(StackFrameTagNames.Line, line)
                .AddValue(StackFrameTagNames.Column, column);

            var tags = tagsBuilder.BuildTags();

            var stackFrameData = new StackFrameData(tags);

            stackFrameData.Type.Should().Be(type);
            stackFrameData.Function.Should().Be(function);
            stackFrameData.File.Should().Be(file);
            stackFrameData.Line.Should().Be(line);
            stackFrameData.Column.Should().Be(column);
        }
    }
}