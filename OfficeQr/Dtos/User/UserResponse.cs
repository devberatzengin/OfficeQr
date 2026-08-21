namespace OfficeQr.Dtos.User;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<string> Roles { get; set; } = new();
}
