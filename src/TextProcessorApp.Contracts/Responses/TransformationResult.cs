using TextProcessorApp.Contracts.Models;

namespace TextProcessorApp.Contracts.Responses;

public record TransformationResult(object ModifiedEntity, Dictionary<string, PropertyMetadata> Metadata);
