using OfficeQr.Entity;

namespace OfficeQr.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    bool IsAdmin();
    Task<User?> GetCurrentUserAsync();
}
