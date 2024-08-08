# Elasticsearch Auto Mapper

## Description
**That makes you can easily auto-map for elasticsearch types.**

## Versions
**Elastic.Clients.Elasticseach => 8.14.9**
**.NET Version => 9.0 Preview 6.0**

## Example of the code:

```csharp
var settings = new ElasticsearchClientSettings(new Uri(""))
            .Authentication(new BasicAuthentication("", ""))
            .PrettyJson()
            .CertificateFingerprint("")
            .DisableDirectStreaming();

var IndexName = "";
var client = new ElasticsearchClient(settings);

await CreateIndexWithMapping(client);

private static async Task CreateIndexWithMapping(ElasticsearchClient client)
{
    var exists = await client.Indices.ExistsAsync(IndexName);
    if (exists.Exists)
        return;
    
    var createIndex = await client.Indices.CreateAsync(IndexName, i =>
    {
        i.Mappings(m =>
        {
            // The model you want to map can be set to T.
            m.Properties(ElasticAutoMapper.MapToElasticIndex<T>()); 
        });
    });
    
    Console.WriteLine(createIndex.IsValidResponse
        ? "Index created successfully with mapping."
        : $"Failed to create index: {createIndex.DebugInformation}");
 }
```

## Custom Attributes:
```csharp
[ElasticsearchCustomMapping] => You can make a custom maps for custom wishes.
[ElasticsearchIgnore] => Using this attribute you can write to models that you do not want to be processed.
[ElasticsearchPropertyName] => Using this attribute you can set the name of the object in your model without SneakCase.
[ElasticsearchStringMapping] => Using this attribute you can say that the object in your model is a TextProperty.
```
