using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Hosting.Abstractions;

namespace Vostok.Logging.Hercules.Configuration
{
    [PublicAPI]
    public static class DefaultHerculesLogProperties
    {
        public static readonly IReadOnlyCollection<string> Blacklisted = new HashSet<string>
        {
            WellKnownProperties.TraceContext
        };

        public static readonly IReadOnlyCollection<string> Whitelisted = new HashSet<string>
        {
            WellKnownProperties.OperationContext,
            WellKnownProperties.SourceContext,
            WellKnownProperties.TraceContext,

            WellKnownApplicationIdentityProperties.Project,
            WellKnownApplicationIdentityProperties.Subproject,
            WellKnownApplicationIdentityProperties.Application,
            WellKnownApplicationIdentityProperties.Environment,
            WellKnownApplicationIdentityProperties.Instance
        };
    }
}