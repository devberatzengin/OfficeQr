using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data.Repositories;

public class CabinetRepository : Repository<Cabinet>, ICabinetRepository
{
    public CabinetRepository (IApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

}