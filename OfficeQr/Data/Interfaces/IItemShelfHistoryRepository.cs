using OfficeQr.Entity;

namespace OfficeQr.Data.Interfaces;

public interface IItemShelfHistoryRepository : IRepository<ItemShelfHistory>
{

    //Custom query method's can add here

    Task<ItemShelfHistory?> GetByShelfId(Guid shelfId, CancellationToken cancellationToken);
    Task<ItemShelfHistory?> GetByItemId(Guid itemId, CancellationToken cancellationToken);
    


}
