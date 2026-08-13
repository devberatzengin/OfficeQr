using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class ShelfRepository : Repository<Shelf>, IShelfRepository
{
    public ShelfRepository (IApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

}