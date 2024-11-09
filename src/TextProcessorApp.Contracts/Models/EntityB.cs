using TextProcessorApp.Contracts.Attributes;
using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Interfaces;

namespace TextProcessorApp.Contracts.Models;
public class EntityB : ITransformable
{
    [TextTransform(TransformType.UpperCase)]
    public string Title { get; set; }

    [TextTransform(TransformType.DashReplacement)]
    [CharacterCount]
    public string Content { get; set; }
}
