namespace OfficeQr.Exceptions;

public class BadRequestException : AppException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;

    public override string Title => "Bad Request";
    public BadRequestException(string message) : base(message)
    {
    }
}
