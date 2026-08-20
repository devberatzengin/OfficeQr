using OfficeQr.Dtos.Common;
using OfficeQr.Dtos.Item;

namespace OfficeQr.Services.Interfaces;

public interface IItemService
{
    Task<Response> CreateAsync(CreateRequest request, CancellationToken cancellationToken);

    Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken);

    Task<Response> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken);

    Task<PagedResponse<Response>> GetAllAsync(ListRequest listRequest, CancellationToken cancellationToken);

    Task<bool> DeleteByIdAsync(Guid itemId, CancellationToken cancellationToken);

    Task<Response> ItemToShelf(Guid itemId, Guid shelfId, Guid? userId, CancellationToken cancellationToken);

    // Task<Response> ReturnItemAsync(ReturnRequest returnRequest, CancellationToken cancellationToken);

    Task<Response> PickupAsync(Guid itemId, CancellationToken cancellationToken);
    Task<Response> ReturnAsync(Guid itemId, Guid shelfId, CancellationToken cancellationToken);

    Task<List<HistoryEntryResponse>> GetHistoryAsync(Guid itemId, CancellationToken cancellationToken);
    Task<List<MyActivityEntryResponse>> GetMyActivityAsync(CancellationToken cancellationToken);
}