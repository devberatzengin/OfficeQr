using OfficeQr.Dtos.Cabinet;

namespace OfficeQr.Services.Interfaces;


public interface ICabinetService
{
    Task<Response> CreateAsync(CreateRequest request, CancellationToken cancellationToken);

    Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken);

    Task<Response> GetCabinetByIdAsync(Guid cabinetId, CancellationToken cancellationToken);

    Task<List<Response>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<Dtos.Shelf.Response>> GetShelvesAsync(Guid cabinetId, CancellationToken cancellationToken);

    Task<bool> DeleteByIdAsync(Guid cabinetId, CancellationToken cancellationToken);
}