namespace OfficeQr.Exceptions;

public class UnauthorizedException : AppException
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;

    public UnauthorizedException(string message) : base(message)
    {
    }
}
