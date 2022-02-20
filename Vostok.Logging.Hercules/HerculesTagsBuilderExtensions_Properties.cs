using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Vostok.Commons.Formatting;
using Vostok.Hercules.Client.Abstractions.Events;

namespace Vostok.Logging.Hercules
{
    internal static class HerculesTagsBuilderExtensions_Properties
    {
        private const int MaximumRecursionDepth = 10;

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

                AddProperty(builder, key, value, formatProvider, 0);
            }

            return builder;
        }

        private static void AddProperty(
            this IHerculesTagsBuilder builder,
            string key,
            object value,
            IFormatProvider formatProvider,
            int depth)
        {
            Type valueType;

            if (builder.TryAddObject(key, value) || depth > MaximumRecursionDepth)
            {
                // return;
            }
            else if (value is IFormattable formattable)
            {
                var format = value is DateTime or DateTimeOffset ? "O" : null;
                builder.AddValue(key, formattable.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            }
            else if (false) // todo (kungurtsev, 20.02.2022): check should not deconstruct
            {
                builder.AddValue(key, ObjectValueFormatter.Format(value, null, formatProvider));
            }
            else if (DictionaryInspector.IsSimpleDictionary(valueType = value.GetType()))
            {
                builder.AddContainer(key,
                    tagsBuilder =>
                    {
                        foreach (var (pKey, pValue) in DictionaryInspector.EnumerateSimpleDictionary(value))
                            tagsBuilder.AddProperty(pKey, pValue, formatProvider, depth + 1);
                    });
            }
            else if (value is IEnumerable enumerable)
            {
                builder.AddVectorOfContainers(key,
                    enumerable.Cast<object>().ToList(),
                    (tagsBuilder, element) =>
                    {
                        if (element != null && ObjectPropertiesExtractor.HasProperties(element.GetType()))
                        {
                            foreach (var (eKey, eValue) in ObjectPropertiesExtractor.ExtractProperties(element))
                                tagsBuilder.AddProperty(eKey, eValue, formatProvider, depth + 1);
                        }
                        else
                        {
                            tagsBuilder.AddProperty("key", element, formatProvider, depth + 1);
                        }
                    });
            }
            else if (ObjectPropertiesExtractor.HasProperties(valueType)) // todo (kungurtsev, 20.02.2022): or check here?
            {
                builder.AddContainer(key,
                    tagsBuilder =>
                    {
                        foreach (var (pKey, pValue) in ObjectPropertiesExtractor.ExtractProperties(value))
                            tagsBuilder.AddProperty(pKey, pValue, formatProvider, depth + 1);
                    });
            }
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