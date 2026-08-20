using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Services;

public class CurrentUserService : ICurrentUserService
{
    private const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    private User? _cachedUser;
    private bool _userLoaded;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public Guid GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var value = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedException("Token'da kullanıcı kimliği bulunamadı.");

        if (!Guid.TryParse(value, out var userId) || userId == Guid.Empty)
        {
            throw new UnauthorizedException("Geçersiz kullanıcı kimliği.");
        }

        return userId;
    }


    public bool IsAdmin()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return false;

        return user.Claims.Any(c =>
            (c.Type == RoleClaimType || c.Type == ClaimTypes.Role) && c.Value == "Admin");
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_userLoaded)
            return _cachedUser;

        var userId = GetCurrentUserId();
        _cachedUser = await _userManager.FindByIdAsync(userId.ToString());
        _userLoaded = true;

        return _cachedUser;
    }
}
