namespace OfficeQr.Dtos.Auth;

public class MeResponse
{

    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
 
}