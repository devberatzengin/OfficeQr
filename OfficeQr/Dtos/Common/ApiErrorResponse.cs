namespace OfficeQr.Dtos.Common;

public class ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public string Message { get; init; } = string.Empty;
}
