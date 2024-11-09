using FluentAssertions;

using NSubstitute;

using System.Text.Json;

using TextProcessorApp.Contracts.Enums;
using TextProcessorApp.Contracts.Exceptions;
using TextProcessorApp.Contracts.Interfaces;
using TextProcessorApp.Core.Extensions;
using TextProcessorApp.Core.Processors;
using TextProcessorApp.UnitTests.TestModels;

namespace TextProcessorApp.UnitTests
{
    public class TransformerServiceTests
    {
        private readonly ITextProcessor _textProcessor;
        private readonly ITransformerService _transformerService;

        public TransformerServiceTests()
        {
            _textProcessor = Substitute.For<ITextProcessor>();
            _transformerService = new TransformerService(_textProcessor);
        }

        [Fact]
        public async Task TransformEntityAsync_WithValidTransformableEntity_ShouldTransformAndReturnResult()
        {
            // Arrange
            var name = "test name";
            var description = "TEST DESCRIPTION";
            var undecorated = "unchanged";

            var transformedName = "TEST NAME";
            var transformedDescription = "test description";


            var entity = new TestTransformableEntity
            {
                Name = name,
                Description = description,
                Undecorated = undecorated
            };

            string jsonPayload = JsonSerializer.Serialize(entity);

            _textProcessor.Transform(name, TransformType.UpperCase).Returns(transformedName);
            _textProcessor.Transform(description, TransformType.LowerCase).Returns(transformedDescription);

            // Act
            var result = await _transformerService.TransformEntityAsync(jsonPayload, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var transformedEntity = result.ModifiedEntity as TestTransformableEntity;
            transformedEntity.Should().NotBeNull();
            transformedEntity!.Name.Should().Be(transformedName);
            transformedEntity.Description.Should().Be(transformedDescription);
            transformedEntity.Undecorated.Should().Be(undecorated);

            result.Metadata.Should().ContainKey("name");
            result.Metadata["name"].CharacterCount.Should().Be(name.Length);
            result.Metadata["name"].Transformations.Should().ContainSingle().Which.Should().Be(TransformType.UpperCase.GetEnumDescription());

            result.Metadata.Should().ContainKey("description");
            result.Metadata["description"].Transformations.Should().ContainSingle().Which.Should().Be(TransformType.LowerCase.GetEnumDescription());

            _textProcessor.Received(1).Transform("test name", TransformType.UpperCase);
            _textProcessor.Received(1).Transform("TEST DESCRIPTION", TransformType.LowerCase);
        }

        [Fact]
        public async Task TransformEntityAsync_WithNullPropertyValues_ShouldHandleGracefully()
        {
            // Arrange
            var entity = new TestTransformableEntity
            {
                Name = null,
                Description = null,
                Undecorated = null
            };
            var jsonPayload = JsonSerializer.Serialize(entity);

            // Act
            var result = await _transformerService.TransformEntityAsync(jsonPayload, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var transformedEntity = result.ModifiedEntity as TestTransformableEntity;
            transformedEntity.Should().NotBeNull();
            transformedEntity!.Name.Should().BeNull();
            transformedEntity.Description.Should().BeNull();
            transformedEntity.Undecorated.Should().BeNull();

            result.Metadata["name"].CharacterCount.Should().BeNull();
            result.Metadata["description"].CharacterCount.Should().BeNull();

            _textProcessor.DidNotReceiveWithAnyArgs().Transform(default!, default);
        }

        [Fact]
        public async Task TransformEntityAsync_WithNonTransformableEntity_ShouldThrowProblemException()
        {
            // Arrange
            var entity = new NonTransformableEntity 
            { 
                SomeName = "test" 
            };

            var jsonPayload = JsonSerializer.Serialize(entity);

            // Act & Assert
            await Assert.ThrowsAsync<ProblemException>(() =>
                _transformerService.TransformEntityAsync(jsonPayload, CancellationToken.None));
        }

        [Fact]
        public async Task TransformEntityAsync_WithInvalidJson_ShouldThrowProblemException()
        {
            // Arrange
            var invalidJson = "{invalid-json}";

            // Act & Assert
            await Assert.ThrowsAsync<ProblemException>(() =>
                _transformerService.TransformEntityAsync(invalidJson, CancellationToken.None));
        }

        [Fact]
        public async Task TransformEntityAsync_WithEmptyJson_ShouldThrowProblemException()
        {
            // Arrange
            var emptyJson = "";

            // Act & Assert
            await Assert.ThrowsAsync<ProblemException>(() =>
                _transformerService.TransformEntityAsync(emptyJson, CancellationToken.None));
        }

        [Fact]
        public async Task TransformEntityAsync_WithUnmappedProperties_ShouldThrowJsonException()
        {
            // Arrange
            var jsonWithExtraProperties = """
            {
                "name": "test",
                "description": "test desc",
                "unknownProperty": "value"
            }
            """;

            // Act & Assert
            await Assert.ThrowsAsync<ProblemException>(() =>
                _transformerService.TransformEntityAsync(jsonWithExtraProperties, CancellationToken.None));
        }

        [Fact]
        public async Task TransformEntityAsync_WithMultipleAttributes_ShouldProcessBothAttributes()
        {
            var testName = "test name";
            var transformedName = "TEST NAME";

            // Arrange
            var entity = new TestTransformableEntity 
            { 
                Name = testName
            };

            string jsonPayload = JsonSerializer.Serialize(entity);

            _textProcessor.Transform(testName, TransformType.UpperCase).Returns(transformedName);

            // Act
            var result = await _transformerService.TransformEntityAsync(jsonPayload, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Metadata["name"].CharacterCount.Should().Be(9);
            result.Metadata["name"].Transformations.Should().ContainSingle().Which.Should().Be(TransformType.UpperCase.GetEnumDescription());
        }
    }
}