using FluentAssertions;

using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Interfaces;
using TextProcessorApp.Core.Processors;

namespace TextProcessorApp.UnitTests;

public class TextProcessorTests
{
    private readonly ITextProcessor _textProvessor;

    public TextProcessorTests()
    {
        _textProvessor = new TextProcessor();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Transform_WhenInputIsNullOrEmpty_ShouldReturnSameInput(string input)
    {
        // Act
        var result = _textProvessor.Transform(input, TransformType.UpperCase);

        // Assert
        result.Should().Be(input);
    }

    [Theory]
    [InlineData("hello world", "HELLO WORLD")]
    [InlineData("Hello World", "HELLO WORLD")]
    [InlineData("HELLO WORLD", "HELLO WORLD")]
    [InlineData("hello123 world!", "HELLO123 WORLD!")]
    public void Transform_WhenTransformTypeIsUpperCase_ShouldConvertToUpperCase(string input, string expected)
    {
        // Act
        var result = _textProvessor.Transform(input, TransformType.UpperCase);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("HELLO WORLD", "hello world")]
    [InlineData("Hello World", "hello world")]
    [InlineData("hello world", "hello world")]
    [InlineData("HELLO123 WORLD!", "hello123 world!")]
    public void Transform_WhenTransformTypeIsLowerCase_ShouldConvertToLowerCase(string input, string expected)
    {
        // Act
        var result = _textProvessor.Transform(input, TransformType.LowerCase);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "hello-world")]
    [InlineData("Hello World", "Hello-World")]
    [InlineData("hello  world", "hello--world")]
    [InlineData("hello world!", "hello-world!")]
    public void Transform_WhenTransformTypeIsDashReplacement_ShouldReplaceSpacesWithDashes(string input, string expected)
    {
        // Act
        var result = _textProvessor.Transform(input, TransformType.DashReplacement);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Transform_WhenTransformTypeIsUndefined_ShouldReturnOriginalInput()
    {
        // Arrange
        var input = "Hello World";
        var invalidTransformType = (TransformType)999;

        // Act
        var result = _textProvessor.Transform(input, invalidTransformType);

        // Assert
        result.Should().Be(input);
    }

    [Theory]
    [InlineData("Hello World", TransformType.UpperCase, "HELLO WORLD")]
    [InlineData("Hello World", TransformType.LowerCase, "hello world")]
    [InlineData("Hello World", TransformType.DashReplacement, "Hello-World")]
    public void Transform_ShouldHandleAllValidTransformTypes(string input, TransformType transformType, string expected)
    {
        // Act
        var result = _textProvessor.Transform(input, transformType);

        // Assert
        result.Should().Be(expected);
    }
}