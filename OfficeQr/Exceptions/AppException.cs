namespace OfficeQr.Exceptions;

public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string Title { get; }

    public virtual string Type =>
        $"https://officeqr.dev/errors/{GetType().Name.Replace("Exception", "").ToLowerInvariant()}";

    protected AppException(string message) : base(message)
    {
    }
}