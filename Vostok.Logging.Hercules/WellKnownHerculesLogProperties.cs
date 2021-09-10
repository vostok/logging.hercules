using JetBrains.Annotations;

namespace Vostok.Logging.Hercules
{
    [PublicAPI]
    public static class WellKnownHerculesLogProperties
    {
        /// <summary>
        /// <para>An optional name of index in Elastic.</para>
        /// <para>If not filled, <c>'project-environment[-subproject]'</c> will be used by default.</para>
        /// </summary>
        public const string ElkIndex = "elk-index";
    }
}