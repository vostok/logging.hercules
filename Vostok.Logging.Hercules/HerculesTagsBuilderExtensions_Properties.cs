using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;

namespace Vostok.Logging.Hercules
{
    internal static class HerculesTagsBuilderExtensions_Properties
    {
        public static IHerculesTagsBuilder AddProperties(
            this IHerculesTagsBuilder builder,
            IReadOnlyDictionary<string, object> properties,
            IReadOnlyCollection<string> filteredProperties,
            IFormatProvider formatProvider)
        {
            foreach (var kvp in properties)
            {
                var (key, value) = (kvp.Key, kvp.Value);
                
                if (IsPositionalName(key))
                    continue;

                if (filteredProperties?.Contains(key) == true)
                    continue;

                if (builder.TryAddObject(key, value))
                    continue;

                var format = value is DateTime or DateTimeOffset ? "O" : null;

                builder.AddValue(key, ObjectValueFormatter.Format(value, format, formatProvider));
            }

            return builder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPositionalName(string propertyName)
        {
            foreach (var character in propertyName)
            {
                if (character < '0')
                    return false;

                if (character > '9')
                    return false;
            }

            return true;
        }
    }
}