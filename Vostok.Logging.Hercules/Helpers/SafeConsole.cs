using System;

namespace Vostok.Logging.Hercules.Helpers
{
    internal static class SafeConsole
    {
        public static void ReportError(string message, Exception error)
        {
            try
            {
                Console.Out.WriteLine("[HerculesLog] " + message);
                Console.Out.WriteLine(error);
            }
            catch
            {
                // ignored
            }
        }
    }
}