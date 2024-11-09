using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using TextProcessorApp.Contracts.Attributes;
using TextProcessorApp.Contracts.Exceptions;
using TextProcessorApp.Contracts.Interfaces;
using TextProcessorApp.Contracts.Models;
using TextProcessorApp.Contracts.Responses;
using TextProcessorApp.Core.Extensions;

namespace TextProcessorApp.Core.Processors;
public class TransformerService : ITransformerService
{
    private readonly ITextProcessor _textProcessor;
    private readonly IEnumerable<Type> _transformableTypes;

    public TransformerService(ITextProcessor textProcessor)
    {
        _textProcessor = textProcessor;
        _transformableTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ITransformable).IsAssignableFrom(t) && !t.IsInterface);
    }

    public async Task<TransformationResult> TransformEntityAsync(string jsonPayload, CancellationToken cancellationToken)
    {
        var serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        foreach (var type in _transformableTypes)
        {
            try
            {
                object? entity = JsonSerializer.Deserialize(jsonPayload, type, serializationOptions);
                
                if (entity is not null)
                {
                    return ProcessEntity(entity);
                }
            }
            catch (Exception)
            {
                continue;
            }
        }

        throw new ProblemException("Validation Exception", "Payload does not match any known transformable entity");
    }

    private TransformationResult ProcessEntity(object entity)
    {
        Dictionary<string, PropertyMetadata> metadata = new Dictionary<string, PropertyMetadata>();
        var properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            var transformAttribute = property.GetCustomAttribute<TextTransformAttribute>();
            var countAttribute = property.GetCustomAttribute<CharacterCountAttribute>();

            if (transformAttribute is null && countAttribute is null)
            {
                continue;
            }

            metadata = ProcessPayloadProperty(entity, metadata, property, transformAttribute, countAttribute);
        }

        return new TransformationResult(entity, metadata);
    }

    private Dictionary<string, PropertyMetadata> ProcessPayloadProperty(
        object entity, 
        Dictionary<string, PropertyMetadata> metadata, 
        PropertyInfo property, 
        TextTransformAttribute transformAttribute, 
        CharacterCountAttribute countAttribute)
    {
        string? value = property.GetValue(entity)?.ToString();

        if (transformAttribute is not null && !string.IsNullOrWhiteSpace(value))
        {
            value = _textProcessor.Transform(value, transformAttribute.TransformType);
            property.SetValue(entity, value);

        }

        int? characterCount = countAttribute != null ? value?.Length : null;
        string[] transformations = transformAttribute != null
                ? new[] { transformAttribute.TransformType.GetEnumDescription() }
                : Array.Empty<string>();

        metadata[property.Name.ToLower()] = new PropertyMetadata(characterCount, transformations);

        return metadata;
    }
}
