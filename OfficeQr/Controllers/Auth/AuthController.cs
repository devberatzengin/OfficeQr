using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Dtos.Auth;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;


    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(AuthRegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(AuthLoginRequest request)
    {
        var principal = await _authService.LoginAsync(request);
        return SignIn(principal, IdentityConstants.BearerScheme);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return SignOut(IdentityConstants.BearerScheme, IdentityConstants.ApplicationScheme);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var email = User.Identity?.Name ?? string.Empty;
        var isAdmin = _currentUserService.IsAdmin();
        var id = _currentUserService.GetCurrentUserId();

        return Ok(new MeResponse { Id = id, Email = email, IsAdmin = isAdmin });
    }


}
