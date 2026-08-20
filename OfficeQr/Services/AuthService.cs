using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OfficeQr.Dtos.Auth;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResponse> RegisterAsync(AuthRegisterRequest request)
    {
        var user = new User { UserName = request.Email, Email = request.Email };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new BadRequestException(errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        return new AuthResponse { Success = true, Email = request.Email, Message = "Registration successful" };
    }

    public async Task<ClaimsPrincipal> LoginAsync(AuthLoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("User account is inactive");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedException("Account is locked out");
            }

            if (result.RequiresTwoFactor)
            {
                throw new UnauthorizedException("Two-factor authentication is required");
            }

            throw new UnauthorizedException("Invalid credentials");
        }

        return await _signInManager.CreateUserPrincipalAsync(user);
    }
}
