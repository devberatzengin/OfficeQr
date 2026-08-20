using System.Text.Json;
using FluentValidation;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Shelf;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Helpers;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Services;


public class ShelfService : IShelfService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ShelfService> _logger;

    private readonly IValidator<CreateRequest> _createRequestValidator;
    private readonly IValidator<UpdateRequest> _updateRequestValidator;

    public ShelfService(
        IUnitOfWork unitOfWork,
        ILogger<ShelfService> logger,
        IValidator<CreateRequest> createRequestValidator,
        IValidator<UpdateRequest> updateRequestValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    public async Task<Response> CreateAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");

        var dbCabinet = await _unitOfWork.Cabinets.GetByIdAsync(request.CabinetId, cancellationToken);

        if (dbCabinet is null)
            throw new NotFoundException($"Cabinet not found with id: {request.CabinetId}");

        if (dbCabinet.Capacity <= 0)
            throw new NoMoreCapacityException($"No more Shelf capacity in cabinet with: {request.CabinetId}");

        var newShelf = new Shelf()
        {
            Id = Guid.NewGuid(),
            QrCode = "",
            Capacity = request.Capacity,
            CabinetId = request.CabinetId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        QrCodeGenerator qrCodeGenerator = new QrCodeGenerator();
        newShelf.QrCode = "data:image/png;base64," + qrCodeGenerator.CreateQr(newShelf);
        dbCabinet.UpdatedOn = DateTime.UtcNow;
        dbCabinet.Capacity -= 1;

        var historyItem = new ShelfCabinetHistory()
        {
            Id = Guid.NewGuid(),
            ShelfId = newShelf.Id,
            CabinetId = request.CabinetId,
            MovedInAt = DateTime.UtcNow
        };

        await _unitOfWork.ShelfCabinetHistories.AddAsync(historyItem, cancellationToken);
        await _unitOfWork.Shelves.AddAsync(newShelf, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Response()
        {
            Id = newShelf.Id,
            QrCode = newShelf.QrCode,
            Capacity = newShelf.Capacity,
            CabinetId = newShelf.CabinetId
        };

    }

    public async Task<bool> DeleteAsync(Guid shelfId, CancellationToken cancellationToken)
    {
        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(shelfId, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {shelfId}");

        var itemsOnShelf = await _unitOfWork.Items.GetByShelfIdAsync(shelfId, cancellationToken);

        if (itemsOnShelf.Count > 0)
            throw new BadRequestException($"Shelf with id {shelfId} still has {itemsOnShelf.Count} item(s) on it. Move them before deleting the shelf.");

        var shelfHistory = await _unitOfWork.ShelfCabinetHistories.GetByShelfId(shelfId, cancellationToken);

        if (shelfHistory is null)
            throw new NotFoundException($"Shelf history not found with id: {shelfId}");

        var dbCabinet = await _unitOfWork.Cabinets.GetByIdAsync(dbShelf.CabinetId, cancellationToken);

        if (dbCabinet is not null)
        {
            dbCabinet.Capacity += 1;
            dbCabinet.UpdatedOn = DateTime.UtcNow;
        }

        dbShelf.IsDeleted = true;
        shelfHistory.MovedOutAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;

    }

    public async Task<Response> GetByIdAsync(Guid shelfId, CancellationToken cancellationToken)
    {
        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(shelfId, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {shelfId}");

        return new Response()
        {
            Id = dbShelf.Id,
            QrCode = dbShelf.QrCode,
            Capacity = dbShelf.Capacity,
            CabinetId = dbShelf.CabinetId
        };
    }

    public async Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");

        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(request.Id, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {request.Id}");

        if (request.Capacity.HasValue)
        {
            dbShelf.Capacity = request.Capacity.Value;
        }

        dbShelf.UpdatedOn = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Response()
        {
            Id = dbShelf.Id,
            QrCode = dbShelf.QrCode,
            Capacity = dbShelf.Capacity,
            CabinetId = dbShelf.CabinetId
        };
    }

    public async Task<List<OfficeQr.Dtos.Item.Response>> GetShelfItemsById(Guid shelfId, CancellationToken cancellationToken)
    {
        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(shelfId, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {shelfId}");

        var items = await _unitOfWork.Items.GetByShelfIdAsync(shelfId, cancellationToken);

        return items.Select(item => new OfficeQr.Dtos.Item.Response
        {
            Id = item.Id,
            QrCode = item.QrCode,
            Name = item.Name,
            UserId = item.UserId,
            ShelfId = item.ShelfId
        }).ToList();
    }

    public async Task<Response> MoveToCabinetAsync(Guid shelfId, Guid newCabinetId, CancellationToken cancellationToken)
    {
        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(shelfId, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {shelfId}");

        if (dbShelf.CabinetId == newCabinetId)
        {
            return new Response()
            {
                Id = dbShelf.Id,
                QrCode = dbShelf.QrCode,
                Capacity = dbShelf.Capacity,
                CabinetId = dbShelf.CabinetId
            };
        }

        var newCabinet = await _unitOfWork.Cabinets.GetByIdAsync(newCabinetId, cancellationToken);

        if (newCabinet is null)
            throw new NotFoundException($"Cabinet not found with id: {newCabinetId}");

        if (newCabinet.Capacity <= 0)
            throw new NoMoreCapacityException($"No more Shelf capacity in cabinet with: {newCabinetId}");

        var now = DateTime.UtcNow;

        var oldCabinet = await _unitOfWork.Cabinets.GetByIdAsync(dbShelf.CabinetId, cancellationToken);
        if (oldCabinet is not null)
        {
            oldCabinet.Capacity += 1;
            oldCabinet.UpdatedOn = now;
        }

        var openHistory = await _unitOfWork.ShelfCabinetHistories.GetByShelfId(shelfId, cancellationToken);
        if (openHistory is not null)
        {
            openHistory.MovedOutAt = now;
            _unitOfWork.ShelfCabinetHistories.Update(openHistory);
        }

        newCabinet.Capacity -= 1;
        newCabinet.UpdatedOn = now;

        await _unitOfWork.ShelfCabinetHistories.AddAsync(new ShelfCabinetHistory
        {
            Id = Guid.NewGuid(),
            ShelfId = shelfId,
            CabinetId = newCabinetId,
            MovedInAt = now
        }, cancellationToken);

        dbShelf.CabinetId = newCabinetId;
        dbShelf.UpdatedOn = now;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Response()
        {
            Id = dbShelf.Id,
            QrCode = dbShelf.QrCode,
            Capacity = dbShelf.Capacity,
            CabinetId = dbShelf.CabinetId
        };
    }

}
