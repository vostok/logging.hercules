using System.Collections.Generic;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;

namespace Vostok.Logging.Hercules
{
    internal static partial class HerculesTagsBuilderExtensions
    {
        public static IHerculesTagsBuilder AddProperties(
            this IHerculesTagsBuilder builder,
            IReadOnlyDictionary<string, object> properties)
        {
            foreach (var keyValuePair in properties)
            {
                if (builder.TryAddPropertyOfHerculesType(keyValuePair.Key, keyValuePair.Value))
                    continue;
                builder.AddValue(keyValuePair.Key, ObjectValueFormatter.Format(keyValuePair.Value));
            }

            return builder;
        }
    }
}