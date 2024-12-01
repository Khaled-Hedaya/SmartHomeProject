public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(string message, List<string> errors = null) : base(message)
    {
        Errors = errors ?? new List<string>();
    }
}