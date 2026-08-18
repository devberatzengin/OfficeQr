namespace OfficeQr.Exceptions;

public class NoMoreCapacityException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public NoMoreCapacityException(string message) : base(message)
    {
    }
}