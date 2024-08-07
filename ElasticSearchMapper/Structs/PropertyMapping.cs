using System.Reflection;
using Elastic.Clients.Elasticsearch.Mapping;

namespace ElasticSearchMapper.Structs;

internal readonly struct PropertyMapping(
    PropertyInfo propertyInfo,
    IProperty elasticProperty,
    string elasticPropertyName)
{
    public readonly PropertyInfo PropertyInfo = propertyInfo;
    public readonly IProperty ElasticProperty = elasticProperty;
    public readonly string ElasticPropertyName = elasticPropertyName;
}