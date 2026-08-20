using OfficeQr.Dtos.Shelf;

namespace OfficeQr.Services.Interfaces;

public interface IShelfService
{
    Task<Response> GetByIdAsync(Guid ShelfId, CancellationToken cancellationToken);

    Task<Response> CreateAsync(CreateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid shelfId, CancellationToken cancellationToken);

    Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken);

    Task<List<OfficeQr.Dtos.Item.Response>> GetShelfItemsById(Guid shelfId, CancellationToken cancellationToken);

    Task<Response> MoveToCabinetAsync(Guid shelfId, Guid newCabinetId, CancellationToken cancellationToken);

}
