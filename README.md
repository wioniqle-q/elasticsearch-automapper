# elasticsearch-automapper

That makes you can easily auto map for elasticsearch types.

## Versions
Elastic.Clients.Elasticseach => 8.14.9

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
            m.Properties(ElasticAutoMapper.MapToElasticIndex<T>()); // You can set the T with you're model.
        });
    });
    
    Console.WriteLine(createIndex.IsValidResponse
        ? "Index created successfully with mapping."
        : $"Failed to create index: {createIndex.DebugInformation}");
 }
```
