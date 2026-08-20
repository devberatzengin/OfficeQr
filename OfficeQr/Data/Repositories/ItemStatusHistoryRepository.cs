using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ItemStatusHistoryRepository : Repository<ItemStatusHistory>, IItemStatusHistoryRepository
{
    public ItemStatusHistoryRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
        
    }
}