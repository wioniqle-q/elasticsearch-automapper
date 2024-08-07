namespace ElasticSearchMapper.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ElasticsearchStringMappingAttribute(bool isKeyword = true) : Attribute
{
    public bool IsKeyword { get; } = isKeyword;
}