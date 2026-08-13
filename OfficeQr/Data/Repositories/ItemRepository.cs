using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository (IApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

}