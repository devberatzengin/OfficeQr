using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository (IApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IReadOnlyList<Item>> GetByShelfIdAsync(Guid shelfId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(i => i.ShelfId == shelfId)
            .ToListAsync(cancellationToken);
    }

}
