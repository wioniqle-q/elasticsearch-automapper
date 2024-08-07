using System.Runtime.CompilerServices;
using Elastic.Clients.Elasticsearch.Mapping;

namespace ElasticSearchMapper.Structs;

internal readonly struct TypeMapping(Properties properties, IReadOnlyList<PropertyMapping> propertyMappings)
{
    public readonly IReadOnlyList<PropertyMapping> PropertyMappings = propertyMappings;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Properties GetProperties() => properties;
}