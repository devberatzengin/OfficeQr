using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ItemUserHistoryRepository : Repository<ItemUserHistory>, IItemUserHistoryRepository
{
    public ItemUserHistoryRepository (IApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<ItemUserHistory?> GetOpenByItemId(Guid itemId, CancellationToken cancellationToken)
    {
        return await Query()
            .Where(iuh => iuh.ItemId == itemId && iuh.ReturnedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
