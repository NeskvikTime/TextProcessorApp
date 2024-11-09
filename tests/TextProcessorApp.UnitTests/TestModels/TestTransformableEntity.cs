using TextProcessorApp.Contracts.Attributes;
using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Interfaces;

namespace TextProcessorApp.UnitTests.TestModels;

public class TestTransformableEntity : ITransformable
{
    [TextTransform(TransformType.UpperCase)]
    [CharacterCount]
    public string? Name { get; set; }

    [TextTransform(TransformType.LowerCase)]
    public string? Description { get; set; }

    public string? Undecorated { get; set; }
}
