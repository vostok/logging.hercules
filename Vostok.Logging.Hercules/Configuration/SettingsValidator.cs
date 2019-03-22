using System;

namespace Vostok.Logging.Hercules.Configuration
{
    internal static class SettingsValidator
    {
        public static HerculesLogSettings ValidateSettings(HerculesLogSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.HerculesSink == null)
                throw new ArgumentNullException(nameof(settings.HerculesSink));

            if (string.IsNullOrWhiteSpace(settings.Stream))
                throw new ArgumentNullException(nameof(settings.Stream));

            if (settings.EnabledLogLevels == null)
                throw new ArgumentNullException(nameof(settings.EnabledLogLevels));

            return settings;
        }
    }
}