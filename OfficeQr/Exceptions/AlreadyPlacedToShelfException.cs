namespace OfficeQr.Exceptions;

public class AlreadyPlacedToShelfException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;

    public override string Title => "Item is already in some shelf.";
    public AlreadyPlacedToShelfException(string message) : base(message)
    {
    }
}
