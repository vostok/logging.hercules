using System;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Hercules
{    
    /// <summary>
    /// <para>A log which send events to a <see cref="IHerculesSink"/>.</para>
    /// </summary>
    public class HerculesLog : ILog
    {
        private readonly Func<HerculesLogSettings> settingsProvider;

        /// <summary>
        /// Create a new <see cref="HerculesLog"/> with given static settings.
        /// </summary>
        public HerculesLog(HerculesLogSettings settings)
            : this(() => settings)
        {
        }
        
        /// <summary>
        /// <para>Create a new Hercules log with the dynamic settings provided by given delegate.</para>
        /// </summary>
        public HerculesLog(Func<HerculesLogSettings> settingsProvider)
            => this.settingsProvider = settingsProvider;

        /// <inheritdoc />
        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;
            
            var settings = settingsProvider();
            
            if (!IsEnabledFor(settings, @event.Level))
                return;
            
            settings.HerculesSink.Put(
                settings.Stream,
                builder => builder.AddLogEventData(@event, settings.FormatProvider));
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level)
            => IsEnabledFor(settingsProvider(), level);

        /// <inheritdoc />
        public ILog ForContext(string context)
            => new SourceContextWrapper(this, context);
        
        private bool IsEnabledFor(HerculesLogSettings settings, LogLevel level)
            => Array.IndexOf(settings.EnabledLogLevels, level) >= 0;
    }
}