using TextProcessorApp.Contracts.Responses;

namespace TextProcessorApp.Contracts.Interfaces;
public interface ITransformerService
{
    Task<TransformationResult> TransformEntityAsync(string jsonRequest, CancellationToken cancellationToken);
}
