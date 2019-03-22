using System;
using Vostok.Logging.Hercules.Helpers;

namespace Vostok.Logging.Hercules.Configuration
{
    internal class SafeSettingsProvider
    {
        private readonly Func<HerculesLogSettings> settingsProvider;

        private HerculesLogSettings cachedSettings;

        public SafeSettingsProvider(Func<HerculesLogSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public HerculesLogSettings Get()
        {
            try
            {
                var actualSettings = settingsProvider();

                if (ReferenceEquals(actualSettings, cachedSettings))
                    return actualSettings;

                return cachedSettings = SettingsValidator.ValidateSettings(actualSettings);
            }
            catch (Exception exception)
            {
                if (cachedSettings == null)
                    throw;

                SafeConsole.ReportError("Failed to update Hercules log configuration:", exception);
                return cachedSettings;
            }
        }
    }
}