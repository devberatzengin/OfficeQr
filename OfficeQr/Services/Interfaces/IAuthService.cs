using System.Security.Claims;
using OfficeQr.Dtos.Auth;

namespace OfficeQr.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(AuthRegisterRequest request);

    Task<ClaimsPrincipal> LoginAsync(AuthLoginRequest request);
}
