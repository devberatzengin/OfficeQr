using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ShelfCabinetHistoryRepository : Repository<ShelfCabinetHistory>, IShelfCabinetHistoryRepository
{
    public ShelfCabinetHistoryRepository (IApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<ShelfCabinetHistory?> GetByShelfId(Guid shelfId, CancellationToken cancellationToken)
    {
        return await Query()
            .Where(sc => sc.ShelfId == shelfId && sc.MovedOutAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
