using OfficeQr.Entity;

namespace OfficeQr.Data.Interfaces;

public interface IItemUserHistoryRepository : IRepository<ItemUserHistory>
{
    Task<ItemUserHistory?> GetOpenByItemId(Guid itemId, CancellationToken cancellationToken);
}
