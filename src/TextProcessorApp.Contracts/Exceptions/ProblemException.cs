namespace TextProcessorApp.Contracts.Exceptions;
public class ProblemException : Exception
{
    public string Error { get; } = default!;

    public override string Message { get; } = default!;

    public ProblemException(string error, string message) : base(error)
    {
        Error = error;
        Message = message;
    }
}
