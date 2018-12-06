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
                case IList<bool> boolList:
                    builder.AddVector(key, boolList);
                    return true;
                case IEnumerable<bool> boolEnumerable:
                    builder.AddVector(key, boolEnumerable.ToArray());
                    return true;
                case byte byteValue:
                    builder.AddValue(key, byteValue);
                    return true;
                case IList<byte> byteList:
                    builder.AddVector(key, byteList);
                    return true;
                case IEnumerable<byte> byteEnumerable:
                    builder.AddVector(key, byteEnumerable.ToArray());
                    return true;
                case short shortValue:
                    builder.AddValue(key, shortValue);
                    return true;
                case IList<short> shortList:
                    builder.AddVector(key, shortList);
                    return true;
                case IEnumerable<short> shortEnumerable:
                    builder.AddVector(key, shortEnumerable.ToArray());
                    return true;
                case int intValue:
                    builder.AddValue(key, intValue);
                    return true;
                case IList<int> intList:
                    builder.AddVector(key, intList);
                    return true;
                case IEnumerable<int> intEnumerable:
                    builder.AddVector(key, intEnumerable.ToArray());
                    return true;
                case long longValue:
                    builder.AddValue(key, longValue);
                    return true;
                case IList<long> longList:
                    builder.AddVector(key, longList);
                    return true;
                case IEnumerable<long> longEnumerable:
                    builder.AddVector(key, longEnumerable.ToArray());
                    return true;
                case float floatValue:
                    builder.AddValue(key, floatValue);
                    return true;
                case IList<float> floatList:
                    builder.AddVector(key, floatList);
                    return true;
                case IEnumerable<float> floatEnumerable:
                    builder.AddVector(key, floatEnumerable.ToArray());
                    return true;
                case double doubleValue:
                    builder.AddValue(key, doubleValue);
                    return true;
                case IList<double> doubleList:
                    builder.AddVector(key, doubleList);
                    return true;
                case IEnumerable<double> doubleEnumerable:
                    builder.AddVector(key, doubleEnumerable.ToArray());
                    return true;
                case Guid GuidValue:
                    builder.AddValue(key, GuidValue);
                    return true;
                case IList<Guid> GuidList:
                    builder.AddVector(key, GuidList);
                    return true;
                case IEnumerable<Guid> GuidEnumerable:
                    builder.AddVector(key, GuidEnumerable.ToArray());
                    return true;
                case string stringValue:
                    builder.AddValue(key, stringValue);
                    return true;
                case IList<string> stringList:
                    builder.AddVector(key, stringList);
                    return true;
                case IEnumerable<string> stringEnumerable:
                    builder.AddVector(key, stringEnumerable.ToArray());
                    return true;
                default:
                    return false;
            }
        }
    }
}