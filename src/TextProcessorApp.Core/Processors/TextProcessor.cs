using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Interfaces;

namespace TextProcessorApp.Core.Processors;
public class TextProcessor : ITextProcessor
{
    public string Transform(string input, TransformType transformType)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return transformType switch
        {
            TransformType.UpperCase => input.ToUpper(),
            TransformType.LowerCase => input.ToLower(),
            TransformType.DashReplacement => input.Replace(' ', '-'),
            _ => input
        };
    }
}