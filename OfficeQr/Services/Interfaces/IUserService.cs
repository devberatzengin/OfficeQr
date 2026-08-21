using OfficeQr.Dtos.User;

namespace OfficeQr.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponse> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserResponse> DeleteAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserResponse> DeactivateAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserResponse> ActivateAsync(Guid userId, CancellationToken cancellationToken);
}
