using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Elastic.Clients.Elasticsearch.Mapping;
using ElasticSearchMapper.Attributes;
using ElasticSearchMapper.Extensions;
using ElasticSearchMapper.Structs;
using TypeMapping = ElasticSearchMapper.Structs.TypeMapping;

namespace ElasticSearchMapper.Managers;

public static class ElasticAutoMapper
{
    private static readonly ConcurrentDictionary<Type, TypeMapping> TypeMappings = new();

    public static Properties MapToElasticIndex<T>() where T : class
    {
        var type = typeof(T);
        var typeMapping = TypeMappings.GetOrAdd(type, CreateTypeMapping);
        return typeMapping.GetProperties();
    }

    public static void GetElasticMappings<T>() where T : class
    {
        var typeMapping = GetTypeMapping<T>();

        foreach (var mapping in typeMapping.PropertyMappings)
            Console.WriteLine(
                $"Get: Property {mapping.PropertyInfo.Name} has [Elastic Property Name] {mapping.ElasticPropertyName} with Type {mapping.ElasticProperty.GetType().Name}");
    }

    public static void GroupPropertiesByType<T>() where T : class
    {
        var typeMapping = GetTypeMapping<T>();

        var groupedProperties = typeMapping.PropertyMappings
            .GroupBy(pm => pm.ElasticProperty.GetType().Name)
            .OrderBy(g => g.Key);

        foreach (var group in groupedProperties)
        {
            var propertyType = group.Key;
            var mappings = group.ToList();

            Console.WriteLine($"Property Type: {propertyType}");
            foreach (var mapping in mappings)
                Console.WriteLine(
                    $" - Property: {mapping.PropertyInfo.Name}, Elastic Property Name: {mapping.ElasticPropertyName}");
        }
    }

    private static TypeMapping GetTypeMapping<T>() where T : class
    {
        return TypeMappings.GetOrAdd(typeof(T), CreateTypeMapping);
    }

    private static Properties MapToElasticIndex(Type type)
    {
        var typeMapping = TypeMappings.GetOrAdd(type, CreateTypeMapping);
        return typeMapping.GetProperties();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TypeMapping CreateTypeMapping(Type type)
    {
        var properties = new Properties();
        var propertyMappings = CreatePropertyMappings(type).ToList();

        foreach (var propertyMapping in propertyMappings)
            properties.Add(propertyMapping.ElasticPropertyName, propertyMapping.ElasticProperty);

        return new TypeMapping(properties, propertyMappings);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<PropertyMapping> CreatePropertyMappings(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => ShouldIgnoreProperty(property) is false);

        foreach (var property in properties)
        {
            var elasticProperty = MapProperty(property);
            var propertyName = GetElasticPropertyName(property);

            yield return new PropertyMapping(property, elasticProperty, propertyName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetElasticPropertyName(PropertyInfo property)
    {
        return property.GetCustomAttribute<ElasticsearchPropertyNameAttribute>()?.Name ??
               property.Name.ToSnakeCase();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ShouldIgnoreProperty(PropertyInfo property)
    {
        return property.GetCustomAttribute<ElasticsearchIgnoreAttribute>() != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IProperty MapProperty(PropertyInfo property)
    {
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (TryGetCustomMapping(property, out var customMapping)) return customMapping;

        return propertyType switch
        {
            _ when propertyType == typeof(string) => CreateStringProperty(property),
            _ when propertyType == typeof(int) || propertyType == typeof(long) => new LongNumberProperty(),
            _ when propertyType == typeof(double) || propertyType == typeof(float) => new DoubleNumberProperty(),
            _ when propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset) => new DateProperty(),
            _ when propertyType == typeof(bool) => new BooleanProperty(),
            _ when propertyType == typeof(Guid) => new KeywordProperty(),
            _ when propertyType == typeof(byte[]) => new BinaryProperty(),
            _ when propertyType == typeof(Uri) => new KeywordProperty(),
            _ when propertyType == typeof(decimal) => new ScaledFloatNumberProperty(),
            _ when propertyType == typeof(byte) || propertyType == typeof(short) || propertyType == typeof(ushort) =>
                new IntegerNumberProperty(),
            _ when propertyType == typeof(char) => new KeywordProperty(),
            _ when propertyType.IsEnum => new KeywordProperty(),
            _ when propertyType == typeof(TimeSpan) => new LongNumberProperty(),
            _ when propertyType == typeof(BigInteger) => new LongNumberProperty(),
            _ when propertyType == typeof(Complex) => new ObjectProperty(),
            _ when propertyType == typeof(TimeOnly) => new DateProperty(),
            _ when propertyType == typeof(DateOnly) => new DateProperty(),
            _ when propertyType == typeof(Half) => new DoubleNumberProperty(),
            _ when propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Tuple<>) =>
                new ObjectProperty(),
            _ when propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ValueTuple<>) =>
                new ObjectProperty(),
            _ when propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>) =>
                CreateListProperty(property),
            _ when propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>) =>
                CreateListProperty(property),
            _ => new ObjectProperty
            {
                Properties = MapToElasticIndex(propertyType)
            }
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetCustomMapping(PropertyInfo property, [NotNullWhen(true)] out IProperty? customMapping)
    {
        var attribute = property.GetCustomAttribute<ElasticsearchCustomMappingAttribute>();
        customMapping = attribute?.CreateMapping();
        return customMapping != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IProperty CreateStringProperty(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<ElasticsearchStringMappingAttribute>();
        return attribute switch
        {
            { IsKeyword: true } => new KeywordProperty(),
            _ => new TextProperty()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IProperty CreateListProperty(PropertyInfo property)
    {
        var elementType = property.PropertyType.GetGenericArguments().First();
        return new ObjectProperty
        {
            Properties = MapToElasticIndex(elementType)
        };
    }
}
