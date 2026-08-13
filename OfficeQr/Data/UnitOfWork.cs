
using OfficeQr.Data.Interfaces;

namespace OfficeQr.Data;

public class UnitOfWork : IUnitOfWork
{

    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IItemRepository _itemRepository;
    private readonly IShelfRepository _shelfRepository;
    private readonly ICabinetRepository _cabinetRepository;

    public UnitOfWork(
        ApplicationDbContext applicationDbContext,
        IItemRepository itemRepository,
        IShelfRepository shelfRepository,
        ICabinetRepository cabinetRepository)
    {
        _applicationDbContext = applicationDbContext;
        _itemRepository = itemRepository;
        _shelfRepository = shelfRepository;
        _cabinetRepository = cabinetRepository;
    }


    
    public IItemRepository Items => _itemRepository;
    public IShelfRepository Shelves => _shelfRepository;
    public ICabinetRepository Cabinets => _cabinetRepository;

    public async ValueTask DisposeAsync()
    {
        await _applicationDbContext.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _applicationDbContext.SaveChangesAsync(ct);
    }
}