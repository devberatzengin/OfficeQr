using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Dtos.User;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UserService(UserManager<User> userManager, IMapper mapper, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        var responses = new List<UserResponse>(users.Count);
        foreach (var user in users)
        {
            responses.Add(await MapWithRolesAsync(user));
        }

        return responses;
    }

    public async Task<UserResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetUserOrThrowAsync(userId);
        return await MapWithRolesAsync(user);
    }

    public async Task<UserResponse> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureNotActingOnSelf(userId, "Kendi hesabını silemezsin.");

        var user = await GetUserOrThrowAsync(userId);

        user.IsDeleted = true;
        user.IsActive = false;
        user.UpdatedOn = DateTime.UtcNow;

        await PersistAsync(user);

        return await MapWithRolesAsync(user);
    }

    public async Task<UserResponse> DeactivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureNotActingOnSelf(userId, "Kendi hesabını pasife alamazsın.");

        var user = await GetUserOrThrowAsync(userId);

        user.IsActive = false;
        user.UpdatedOn = DateTime.UtcNow;

        await PersistAsync(user);

        return await MapWithRolesAsync(user);
    }

    public async Task<UserResponse> ActivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetUserOrThrowAsync(userId);

        user.IsActive = true;
        user.UpdatedOn = DateTime.UtcNow;

        await PersistAsync(user);

        return await MapWithRolesAsync(user);
    }

    // Select kısmını ayrımak doğru sektör standartıymış
    private async Task<User> GetUserOrThrowAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null || user.IsDeleted)
        {
            throw new NotFoundException($"User not found with {userId} id");
        }

        return user;
    }

    // değiştirmeye çalıştığım kişi ben miyim kontrolünü burda yapıyorumö
    private void EnsureNotActingOnSelf(Guid userId, string message)
    {
        if (_currentUserService.GetCurrentUserId() == userId)
        {
            throw new BadRequestException(message);
        }
    }

    // private update handler
    private async Task PersistAsync(User user)
    {
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new BadRequestException(errors);
        }
    }


    // mapper handler
    private async Task<UserResponse> MapWithRolesAsync(User user)
    {
        var response = _mapper.Map<UserResponse>(user);
        response.Roles = (await _userManager.GetRolesAsync(user)).ToList();
        return response;
    }
}
