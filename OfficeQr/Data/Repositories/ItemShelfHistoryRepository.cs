using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ItemShelfHistoryRepository : Repository<ItemShelfHistory>, IItemShelfHistoryRepository
{
    public ItemShelfHistoryRepository (IApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<ItemShelfHistory?> GetByItemId(Guid itemId, CancellationToken cancellationToken)
    {
        return await Query()
            .Where(ish => ish.ItemId == itemId && ish.RemovedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ItemShelfHistory?> GetByShelfId(Guid shelfId, CancellationToken cancellationToken)
    {
        return await Query()
            .Where(ish => ish.ShelfId == shelfId && ish.RemovedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
