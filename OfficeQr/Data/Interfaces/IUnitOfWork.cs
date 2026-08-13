
namespace OfficeQr.Data.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    // Buraya Sırayla Repository'ler Eklenicek
    IItemRepository Items {get;}
    IShelfRepository Shelves {get;}
    ICabinetRepository Cabinets {get;}

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
