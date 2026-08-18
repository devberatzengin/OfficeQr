using OfficeQr.Entity;

namespace OfficeQr.Data.Interfaces;

public interface IShelfCabinetHistoryRepository : IRepository<ShelfCabinetHistory>
{

    Task<ShelfCabinetHistory?> GetByShelfId(Guid shelfId, CancellationToken cancellationToken);

}
