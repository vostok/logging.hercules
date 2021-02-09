using System.Text.RegularExpressions;

namespace Vostok.Logging.Hercules.Helpers
{
    internal static class ExceptionsNormalizer
    {
        private static readonly Regex GuidRegex = new Regex("[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static string Normalize(string value) =>
            string.IsNullOrEmpty(value) ? value : GuidRegex.Replace(value, "~");
    }
}