using Elastic.Clients.Elasticsearch.Mapping;

namespace ElasticSearchMapper.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public abstract class ElasticsearchCustomMappingAttribute : Attribute
{
    public abstract IProperty CreateMapping();
}