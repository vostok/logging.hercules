using System;
using System.Linq;
using System.Collections.Generic;
using Vostok.Hercules.Client.Abstractions.Events;

namespace Vostok.Logging.Hercules
{
    internal static partial class HerculesTagsBuilderExtensions
    {
        private static bool TryAddPropertyOfHerculesType(this IHerculesTagsBuilder builder, string key, object value)
        {
            switch (value)
            {
                case bool boolValue:
                    builder.AddValue(key, boolValue);
                    return true;

                case IEnumerable<bool> boolEnumerable:
                    if (boolEnumerable is IReadOnlyList<bool> boolList)
                        builder.AddVector(key, boolList);
                    else
                        builder.AddVector(key, boolEnumerable.ToArray());
                    return true;

                case byte byteValue:
                    builder.AddValue(key, byteValue);
                    return true;

                case IEnumerable<byte> byteEnumerable:
                    if (byteEnumerable is IReadOnlyList<byte> byteList)
                        builder.AddVector(key, byteList);
                    else
                        builder.AddVector(key, byteEnumerable.ToArray());
                    return true;

                case short shortValue:
                    builder.AddValue(key, shortValue);
                    return true;

                case IEnumerable<short> shortEnumerable:
                    if (shortEnumerable is IReadOnlyList<short> shortList)
                        builder.AddVector(key, shortList);
                    else
                        builder.AddVector(key, shortEnumerable.ToArray());
                    return true;

                case int intValue:
                    builder.AddValue(key, intValue);
                    return true;

                case IEnumerable<int> intEnumerable:
                    if (intEnumerable is IReadOnlyList<int> intList)
                        builder.AddVector(key, intList);
                    else
                        builder.AddVector(key, intEnumerable.ToArray());
                    return true;

                case long longValue:
                    builder.AddValue(key, longValue);
                    return true;

                case IEnumerable<long> longEnumerable:
                    if (longEnumerable is IReadOnlyList<long> longList)
                        builder.AddVector(key, longList);
                    else
                        builder.AddVector(key, longEnumerable.ToArray());
                    return true;

                case float floatValue:
                    builder.AddValue(key, floatValue);
                    return true;

                case IEnumerable<float> floatEnumerable:
                    if (floatEnumerable is IReadOnlyList<float> floatList)
                        builder.AddVector(key, floatList);
                    else
                        builder.AddVector(key, floatEnumerable.ToArray());
                    return true;

                case double doubleValue:
                    builder.AddValue(key, doubleValue);
                    return true;

                case IEnumerable<double> doubleEnumerable:
                    if (doubleEnumerable is IReadOnlyList<double> doubleList)
                        builder.AddVector(key, doubleList);
                    else
                        builder.AddVector(key, doubleEnumerable.ToArray());
                    return true;

                case Guid GuidValue:
                    builder.AddValue(key, GuidValue);
                    return true;

                case IEnumerable<Guid> GuidEnumerable:
                    if (GuidEnumerable is IReadOnlyList<Guid> GuidList)
                        builder.AddVector(key, GuidList);
                    else
                        builder.AddVector(key, GuidEnumerable.ToArray());
                    return true;

                case string stringValue:
                    builder.AddValue(key, stringValue);
                    return true;

                case IEnumerable<string> stringEnumerable:
                    if (stringEnumerable is IReadOnlyList<string> stringList)
                        builder.AddVector(key, stringList);
                    else
                        builder.AddVector(key, stringEnumerable.ToArray());
                    return true;

                default:
                    return false;
            }
        }
    }
}