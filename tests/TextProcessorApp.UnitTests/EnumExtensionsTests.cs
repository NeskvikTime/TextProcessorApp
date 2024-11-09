using FluentAssertions;

using System.ComponentModel;

using TextProcessorApp.Core.Extensions;

namespace TextProcessorApp.UnitTests;

public class EnumExtensionsTests
{
    public enum TestEnumWithDescription
    {
        [Description("First Value Description")]
        FirstValue,

        [Description("Second Value Description")]
        SecondValue,

        [Description("")]
        EmptyDescription,

        NoDescription
    }

    public enum TestEnumWithoutDescription
    {
        FirstValue,
        SecondValue
    }

    [Fact]
    public void GetEnumDescription_WhenEnumHasDescription_ShouldReturnDescription()
    {
        // Arrange
        var enumValue = TestEnumWithDescription.FirstValue;

        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().Be("First Value Description");
    }

    [Fact]
    public void GetEnumDescription_WhenEnumHasNoDescription_ShouldReturnEnumValueName()
    {
        // Arrange
        var enumValue = TestEnumWithDescription.NoDescription;

        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().Be("NoDescription");
    }

    [Fact]
    public void GetEnumDescription_WhenEnumHasEmptyDescription_ShouldReturnEmptyString()
    {
        // Arrange
        var enumValue = TestEnumWithDescription.EmptyDescription;

        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(TestEnumWithoutDescription.FirstValue, "FirstValue")]
    [InlineData(TestEnumWithoutDescription.SecondValue, "SecondValue")]
    public void GetEnumDescription_WhenEnumTypeHasNoDescriptions_ShouldReturnEnumValueName(
        TestEnumWithoutDescription enumValue,
        string expected)
    {
        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetEnumDescription_WhenCalledMultipleTimes_ShouldReturnSameResult()
    {
        // Arrange
        var enumValue = TestEnumWithDescription.FirstValue;

        // Act
        var firstCall = enumValue.GetEnumDescription();
        var secondCall = enumValue.GetEnumDescription();
        var thirdCall = enumValue.GetEnumDescription();

        // Assert
        firstCall.Should().Be("First Value Description");
        secondCall.Should().Be(firstCall);
        thirdCall.Should().Be(firstCall);
    }

    private enum TestEnumWithMultipleAttributes
    {
        [Description("Visible Description")]
        [Obsolete("Obsolete Message")]
        ValueWithMultipleAttributes
    }

    [Fact]
    public void GetEnumDescription_WhenEnumHasMultipleAttributes_ShouldReturnDescriptionAttributeValue()
    {
        // Arrange
        var enumValue = TestEnumWithMultipleAttributes.ValueWithMultipleAttributes;

        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().Be("Visible Description");
    }

    [Fact]
    public void GetEnumDescription_WithDifferentEnumTypes_ShouldWorkConsistently()
    {
        // Arrange & Act
        var result1 = TestEnumWithDescription.FirstValue.GetEnumDescription();
        var result2 = TestEnumWithoutDescription.FirstValue.GetEnumDescription();

        // Assert
        result1.Should().Be("First Value Description");
        result2.Should().Be("FirstValue");
    }

    public enum TestEnumWithUnicode
    {
        [Description("描述")]
        UnicodeDescription,

        [Description("🌟 Star")]
        EmojiDescription
    }

    [Theory]
    [InlineData(TestEnumWithUnicode.UnicodeDescription, "描述")]
    [InlineData(TestEnumWithUnicode.EmojiDescription, "🌟 Star")]
    public void GetEnumDescription_WithUnicodeDescriptions_ShouldReturnCorrectDescription(
        TestEnumWithUnicode enumValue,
        string expected)
    {
        // Act
        var result = enumValue.GetEnumDescription();

        // Assert
        result.Should().Be(expected);
    }
}