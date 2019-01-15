using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Hercules.Configuration;

namespace Vostok.Logging.Hercules.Tests.Configuration
{
    public class SettingsValidator_Tests
    {
        private readonly IHerculesSink nullSink = new DevNullHerculesSink();
        private const string Stream = nameof(Stream);

        [Test]
        public void ValidateSettings_should_not_allow_null_EnabledLogLevels()
        {            
            var settings = new HerculesLogSettings(nullSink, Stream) {EnabledLogLevels = null};
            
            new Action(() => SettingsValidator.ValidateSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateSettings_should_not_allow_null_HerculesSink()
        {            
            var settings = new HerculesLogSettings(nullSink, Stream) {HerculesSink = null};
            
            new Action(() => SettingsValidator.ValidateSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateSettings_should_not_allow_null_Stream()
        {            
            var settings = new HerculesLogSettings(nullSink, Stream) {Stream = null};
            
            new Action(() => SettingsValidator.ValidateSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateSettings_should_not_allow_empty_Stream()
        {            
            var settings = new HerculesLogSettings(nullSink, Stream) {Stream = string.Empty};
            
            new Action(() => SettingsValidator.ValidateSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateSettings_should_pass_valid_settings()
        {            
            var settings = new HerculesLogSettings(nullSink, Stream);
            
            SettingsValidator.ValidateSettings(settings).Should().Be(settings);
        }
    }
}