namespace TextProcessorApp.API.Extensions;

public static class RequestExtensions
{
    public static async Task<string> ReadAsStringAsync(this Stream requestBody, CancellationToken cancellationToken, bool leaveOpen = false)
    {
        string bodyAsString;
        using (StreamReader reader = new(requestBody, leaveOpen: leaveOpen))
        {
            bodyAsString = await reader.ReadToEndAsync(cancellationToken);
        }

        return bodyAsString;
    }
}
