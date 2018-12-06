using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Hercules
{
    /// <summary>
    /// <para>Settings of a <see cref="HerculesLog"/> instance.</para>
    /// <para><see cref="HerculesLogSettings"/> instances should be treated as immutable after construction:
    /// to reconfigure a <see cref="HerculesLog"/> on the fly, always create a new one instead of modifying the properties directly.</para>
    /// </summary>
    public class HerculesLogSettings
    {
        [NotNull]
        public IHerculesSink HerculesSink { get; set; }
        
        [NotNull]
        public string Stream { get; set; }
        
        /// <summary>
        /// <para>If specified, this <see cref="IFormatProvider"/> will be used when formatting log property values.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: it's accessed for each <see cref="LogEvent"/>.</para>
        /// </summary>
        [CanBeNull]
        public IFormatProvider FormatProvider { get; set; }
        
        /// <summary>
        /// <para>A whitelist of enabled <see cref="LogLevel"/>s (contains all levels by default). Only log events with levels contained in this array will be logged.</para>
        /// <para>Dynamic reconfiguration is supported for this parameter: it's accessed for each <see cref="LogEvent"/>.</para>
        /// </summary>
        [NotNull]
        public LogLevel[] EnabledLogLevels { get; set; } = (LogLevel[])Enum.GetValues(typeof(LogLevel));
    }
}