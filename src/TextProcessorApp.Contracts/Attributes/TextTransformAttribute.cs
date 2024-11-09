using TextProcessorApp.Contracts.Enums;

namespace TextProcessorApp.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TextTransformAttribute : Attribute
{
    public TransformType TransformType { get; }

    public TextTransformAttribute(TransformType transformType)
    {
        TransformType = transformType;
    }
}
