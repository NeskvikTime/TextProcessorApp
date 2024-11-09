using Microsoft.AspNetCore.Http.Features;

using TextProcessorApp.API.Extensions;
using TextProcessorApp.API.Middlewares;
using TextProcessorApp.Contracts.Interfaces;
using TextProcessorApp.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCoreServices();

builder.Services.AddProblemDetails(options =>
{
    // RFC 9457
    // Link: https://www.rfc-editor.org/rfc/rfc9457.html
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        context.ProblemDetails.Extensions.TryAdd("traceId", activity);
    };
});

builder.Services.AddExceptionHandler<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.MapPost("/api/transform", async (HttpRequest request, ITransformerService transformerService, CancellationToken cancellationToken) =>
{
   
    string jsonRequest = await request.Body.ReadAsStringAsync(cancellationToken);
    var result = await transformerService.TransformEntityAsync(jsonRequest, cancellationToken);
    return Results.Ok(result);

})
.WithName("Transform")
.WithOpenApi();

app.UseHttpsRedirection();

app.Run();
