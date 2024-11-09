using Microsoft.Extensions.DependencyInjection;

using TextProcessorApp.Contracts.Interfaces;
using TextProcessorApp.Core.Processors;

namespace TextProcessorApp.Core;
public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.AddSingleton<ITextProcessor, TextProcessor>();
        serviceDescriptors.AddScoped<ITransformerService, TransformerService>();

        return serviceDescriptors;
    }
}
