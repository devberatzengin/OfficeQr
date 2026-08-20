using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Exceptions;

namespace OfficeQr.Data;

public class UnitOfWork : IUnitOfWork
{

    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IItemRepository _itemRepository;
    private readonly IShelfRepository _shelfRepository;
    private readonly ICabinetRepository _cabinetRepository;
    private readonly IItemShelfHistoryRepository _itemShelfHistoryRepository;
    private readonly IItemUserHistoryRepository _itemUserHistoryRepository;
    private readonly IShelfCabinetHistoryRepository _shelfCabinetHistoryRepository;
    private readonly IItemStatusHistoryRepository _itemStatusHistoryRepository;

    public UnitOfWork(
        ApplicationDbContext applicationDbContext,
        IItemRepository itemRepository,
        IShelfRepository shelfRepository,
        ICabinetRepository cabinetRepository,
        IItemShelfHistoryRepository itemShelfHistoryRepository,
        IItemUserHistoryRepository itemUserHistoryRepository,
        IShelfCabinetHistoryRepository shelfCabinetHistoryRepository,
        IItemStatusHistoryRepository itemStatusHistoryRepository)
    {
        _applicationDbContext = applicationDbContext;
        _itemRepository = itemRepository;
        _shelfRepository = shelfRepository;
        _cabinetRepository = cabinetRepository;
        _itemShelfHistoryRepository = itemShelfHistoryRepository;
        _itemUserHistoryRepository = itemUserHistoryRepository;
        _shelfCabinetHistoryRepository = shelfCabinetHistoryRepository;
        _itemStatusHistoryRepository = itemStatusHistoryRepository;
    }



    public IItemRepository Items => _itemRepository;
    public IShelfRepository Shelves => _shelfRepository;
    public ICabinetRepository Cabinets => _cabinetRepository;
    public IItemShelfHistoryRepository ItemShelfHistories => _itemShelfHistoryRepository;
    public IItemUserHistoryRepository ItemUserHistories => _itemUserHistoryRepository;
    public IShelfCabinetHistoryRepository ShelfCabinetHistories => _shelfCabinetHistoryRepository;
    public IItemStatusHistoryRepository ItemStatusHistories => _itemStatusHistoryRepository;

    public async ValueTask DisposeAsync()
    {
        await _applicationDbContext.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _applicationDbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "Bu ürün senden hemen önce başka biri tarafından değiştirildi. Lütfen sayfayı yenileyip tekrar dene.");
        }
    }

}
