using TextProcessorApp.Contracts.Attributes;
using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Interfaces;

namespace TextProcessorApp.Contracts.Models;
public class EntityA : ITransformable
{
    [TextTransform(TransformType.LowerCase)]
    [CharacterCount]
    public string Name { get; set; }

    public string Description { get; set; }
}
