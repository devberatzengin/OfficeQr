
namespace OfficeQr.Data.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    // Buraya Sırayla Repository'ler Eklenicek
    IItemRepository Items {get;}
    IShelfRepository Shelves {get;}
    ICabinetRepository Cabinets {get;}

    IItemShelfHistoryRepository ItemShelfHistories {get;}
    IItemUserHistoryRepository ItemUserHistories {get;}
    IShelfCabinetHistoryRepository ShelfCabinetHistories {get;}

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
