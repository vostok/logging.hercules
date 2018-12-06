using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Queries;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Hercules
{
    public class HerculesLog : ILog
    {
        private readonly Func<HerculesLogSettings> settingsProvider;
        private readonly IHerculesSink herculesSink;

        public HerculesLog(HerculesLogSettings settings)
            : this(() => settings)
        {
        }
        
        public HerculesLog(Func<HerculesLogSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public void Log(LogEvent @event)
        {
            var settings = settingsProvider();
            
            if (!IsEnabledFor(settings, @event.Level))
                return;
            
            settings.HerculesSink.Put(settings.Stream,
                builder =>
                {
                    builder
                        .SetTimestamp(@event.Timestamp)
                        .AddValue("TimeZone", @event.Timestamp.Offset.Ticks)
                        .AddValue("MessageTemplate", @event.MessageTemplate ?? string.Empty)
                        .AddValue("RenderedMessage", LogMessageFormatter.Format(@event, settings.FormatProvider))
                        .AddContainer("Properties", tagsBuilder => tagsBuilder.AddProperties(@event.Properties));
                });
        }

        public bool IsEnabledFor(LogLevel level) => IsEnabledFor(settingsProvider(), level);

        public ILog ForContext(string context) => /* TODO*/ this;
        
        private bool IsEnabledFor(HerculesLogSettings settings, LogLevel level) =>
            Array.IndexOf(settings.EnabledLogLevels, level) >= 0;
    }
}