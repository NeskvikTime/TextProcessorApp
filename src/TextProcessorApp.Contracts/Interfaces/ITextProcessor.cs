using TextProcessorApp.Contracts.Enums;

namespace TextProcessorApp.Contracts.Interfaces;
public interface ITextProcessor
{
    string Transform(string input, TransformType transformType);
}

