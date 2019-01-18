using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Hercules.Configuration;

namespace Vostok.Logging.Hercules
{
    /// <summary>
    /// A log which sends events to a <see cref="IHerculesSink"/>.
    /// </summary>
    [PublicAPI]
    public class HerculesLog : ILog
    {
        private readonly SafeSettingsProvider settingsProvider;

        /// <summary>
        /// Create a new <see cref="HerculesLog"/> with given static settings.
        /// </summary>
        public HerculesLog(HerculesLogSettings settings)
            : this(() => settings)
        {
        }

        /// <summary>
        /// Create a new Hercules log with the dynamic settings provided by given delegate.
        /// </summary>
        public HerculesLog(Func<HerculesLogSettings> settingsProvider)
        {
            this.settingsProvider = new SafeSettingsProvider(settingsProvider);
            this.settingsProvider.Get();
        }

        /// <inheritdoc />
        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;
            
            var settings = settingsProvider.Get();
            
            if (!IsEnabledFor(settings, @event.Level))
                return;
            
            settings.HerculesSink.Put(
                settings.Stream,
                builder => builder.AddLogEventData(@event, settings.FormatProvider));
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level)
            => IsEnabledFor(settingsProvider.Get(), level);

        /// <inheritdoc />
        public ILog ForContext(string context)
            => new SourceContextWrapper(this, context);
        
        private bool IsEnabledFor(HerculesLogSettings settings, LogLevel level)
            => Array.IndexOf(settings.EnabledLogLevels, level) >= 0;
    }
}