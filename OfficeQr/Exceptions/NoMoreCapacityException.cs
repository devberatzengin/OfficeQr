namespace OfficeQr.Exceptions;

public class NoMoreCapacityException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;

    public override string Title => "No More Capacity";

    public NoMoreCapacityException(string message) : base(message)
    {
    }
}