
using OfficeQr.Entity;

namespace OfficeQr.Data.Interfaces;

public interface IItemRepository : IRepository<Item>
{

    Task<IReadOnlyList<Item>> GetByShelfIdAsync(Guid shelfId, CancellationToken cancellationToken = default);

}
