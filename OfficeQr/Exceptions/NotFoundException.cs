namespace OfficeQr.Exceptions;

public class NotFoundException : AppException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public NotFoundException(string message) : base(message)
    {
    }
}