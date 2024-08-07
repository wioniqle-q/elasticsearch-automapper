namespace ElasticSearchMapper.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ElasticsearchPropertyNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}