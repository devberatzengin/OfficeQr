namespace OfficeQr.Exceptions;

public class AlreadyInUseException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;

    public override string Title => "Item is already in use";
    public AlreadyInUseException(string message) : base(message)
    {
    }
}
