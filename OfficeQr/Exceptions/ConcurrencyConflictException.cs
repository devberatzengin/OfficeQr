namespace OfficeQr.Exceptions;

public class ConcurrencyConflictException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Concurrency Conflict";

    public ConcurrencyConflictException(string message) : base(message)
    {
    }
}