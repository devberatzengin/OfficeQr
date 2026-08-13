
namespace OfficeQr.Dtos.Auth;


public class AuthResponse
{
        public bool Success { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
}