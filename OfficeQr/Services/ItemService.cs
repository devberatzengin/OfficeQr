using System.Text.Json;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos;
using OfficeQr.Dtos.Common;
using OfficeQr.Dtos.Item;
using OfficeQr.Entity;
using OfficeQr.Entity.Enums;
using OfficeQr.Exceptions;
using OfficeQr.Helpers;
using OfficeQr.Services.Interfaces;
using QRCoder;


namespace OfficeQr.Services;

public class ItemService : IItemService
{

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ItemService> _logger;

    private readonly IValidator<CreateRequest> _createRequestValidator;
    private readonly IValidator<ReturnRequest> _returnRequestValidator;
    private readonly IValidator<UpdateRequest> _updateRequestValidator;

    private readonly ICurrentUserService  _currentUserService;
    public ItemService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<ItemService> logger,
        IValidator<ReturnRequest> returnRequestValidator,
        IValidator<CreateRequest> createRequestValidator,
        IValidator<UpdateRequest> updateRequestValidator,
        ICurrentUserService currentUserService)
    {
        _returnRequestValidator = returnRequestValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _currentUserService = currentUserService;
    }

    public async Task<Response> CreateAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");

        var dbShelf = await _unitOfWork.Shelves.GetByIdAsync(request.ShelfId, cancellationToken);

        if (dbShelf is null)
            throw new NotFoundException($"Shelf not found with id : {request.ShelfId}");

        if (dbShelf.Capacity <= 0)
            throw new NoMoreCapacityException($"No More capacity at shelf with id : {request.ShelfId}. Current capacity : {dbShelf.Capacity}");

        var now = DateTime.UtcNow;
        var currentUserId = _currentUserService.GetCurrentUserId();

        Item item = new Item()
        {
            Id = Guid.NewGuid(),
            // UserId = currentUserId, // İtem Oluşturulurken UserId kısmı şuan kullanan kuşalnıcı id'si olamlı moruk
            ShelfId = request.ShelfId,
            QrCode = "Not Assigned Yet",
            Name = request.Name,
            CreatedOn = now,
            UpdatedOn = now,
            IsActive = true,
            IsDeleted = false
        };

        QrCodeGenerator qrCodeGenerator = new QrCodeGenerator();
        item.QrCode = "data:image/png;base64," + qrCodeGenerator.CreateQr(item);

        dbShelf.Capacity -= 1;
        dbShelf.UpdatedOn = now;

        await ChangeStatusAsync(item, ItemStatus.Available, currentUserId, cancellationToken);


        var newShelfHistory = new ItemShelfHistory()
        {
            Id = Guid.NewGuid(),
            ItemId = item.Id,
            ShelfId = request.ShelfId,
            PlacedAt = now,
            PlacedByUserId = currentUserId,
            Reason = ItemMovementReason.Created
        };

        /*
        var newUserHistory = new ItemUserHistory()
        {
            Id = Guid.NewGuid(),
            ItemId = item.Id,
            UserId = currentUserId
        };
        */
        
        await _unitOfWork.ItemShelfHistories.AddAsync(newShelfHistory, cancellationToken);
        // await _unitOfWork.ItemUserHistories.AddAsync(newUserHistory, cancellationToken); // Burda atandı veya bırakıldı diye tutumamız gerekiyordu Ben malım
        await _unitOfWork.Items.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(item);
    }
    public async Task<bool> DeleteByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var dbResult = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);
        if (dbResult is null)
            throw new NotFoundException($"Item not found with {itemId} id");

        var currentUserId = _currentUserService.GetCurrentUserId();

        await ChangeStatusAsync(dbResult, ItemStatus.Disposed, currentUserId, cancellationToken);
        await MoveItemAsync(dbResult, newShelfId: null, newUserId: null, ItemMovementReason.Removed, cancellationToken);

        dbResult.IsDeleted = true;
        dbResult.IsActive = false;
        dbResult.UpdatedOn = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResponse<Response>> GetAllAsync(ListRequest listRequest, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, listRequest.Page);
        var pageSize = Math.Clamp(listRequest.PageSize, 1, 100);

        var items = await _unitOfWork.Items.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(listRequest.Search))
        {
            var pattern = listRequest.Search.Trim();
            items = items.Where(a => a.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalCount = items.Count;
        var pagedItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        _logger.LogInformation("Fetched {Count} items (page {Page}, size {PageSize})", totalCount, page, pageSize);

        return new PagedResponse<Response>
        {
            Items = _mapper.Map<List<Response>>(pagedItems),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    public async Task<Response> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);

        if (item is null)
            throw new NotFoundException($"Item not found with {itemId} id");


        return _mapper.Map<Response>(item);

    }
    public async Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");

        var dbItem = await _unitOfWork.Items.GetByIdAsync(request.Id, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item not found with {request.Id}");

        if (request.Name is not null)
            dbItem.Name = request.Name;

        var targetShelfId = request.ShelfId ?? dbItem.ShelfId;
        var targetUserId = request.UserId ?? dbItem.UserId;

        await MoveItemAsync(dbItem, targetShelfId, targetUserId, ItemMovementReason.Updated, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(dbItem); 

    }

    public async Task<Response> ItemToShelf(Guid itemId, Guid shelfId, Guid? userId, CancellationToken cancellationToken)
    {
        var dbItem = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item not found with {itemId}");

        var targetUserId = userId ?? dbItem.UserId;

        await MoveItemAsync(dbItem, shelfId, targetUserId, ItemMovementReason.Moved,  cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(dbItem); 

    }

    /*
    public async Task<Response> ReturnItemAsync(ReturnRequest returnRequest, CancellationToken cancellationToken)
    {
        var validationResult = await _returnRequestValidator.ValidateAsync(returnRequest, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");

        var dbItem = await _unitOfWork.Items.GetByIdAsync(returnRequest.ItemId, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item Not found with id: {returnRequest.ItemId}");

        var dbItemUser = await _unitOfWork.ItemUserHistories.GetOpenByItemId(returnRequest.ItemId, cancellationToken);

        if (dbItemUser is null)
            throw new AlreadyInUseException($"Item with id: {returnRequest.ItemId} is already in use by someone");
        
        var dbItemShelf = await _unitOfWork.ItemShelfHistories.GetByItemId(returnRequest.ItemId, cancellationToken);
        if (dbItemShelf is null)
        {   
            
            // Geçmişte bu rafa bir kere konmuş oalbilir o zamanda o değerin removedAt değeri olmalı. 
            // Burda kaydı olamyabailir o zaman yeni oluşturacağız buda ilk kez konmuş demek olucak
            throw new AlreadyPlacedToShelfException($"Item with id : {returnRequest.ItemId} is already in shelf with id: {dbItemShelf.ShelfId}");
        }


        //TODO:     
        
        //        Moruk şimdi Shelfin capcity'i 1 arttır.
        //        Item Table UserID kısmını null'la va shelf'ini gelen shelfId'i yaz abi
        //        Item User History Table'a Returned At'i UtcNow'la
        //        Item Shelf History'de verisi yoksa ekle ve PlacedAt'i now yap
        
        throw new NotImplementedException();
    }

    */


    public async Task<Response> PickupAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var dbItem = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item not found with {itemId}");

        if (dbItem.Status != ItemStatus.Available)
            throw new BadRequestException($"Item is not available for pickup (current status: {dbItem.Status}).");

        var currentUserId = _currentUserService.GetCurrentUserId();

        await ChangeStatusAsync(dbItem, ItemStatus.InUse, currentUserId, cancellationToken);
        await MoveItemAsync(dbItem, dbItem.ShelfId, currentUserId, ItemMovementReason.PickedUp, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(dbItem);
    }

    public async Task<Response> ReturnAsync(Guid itemId, Guid shelfId, CancellationToken cancellationToken)
    {
        var dbItem = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item not found with {itemId}");

        if (dbItem.Status != ItemStatus.InUse)
            throw new BadRequestException($"Item is not currently in use (current status: {dbItem.Status}).");

        var currentUserId = _currentUserService.GetCurrentUserId();

        await ChangeStatusAsync(dbItem, ItemStatus.Available, currentUserId, cancellationToken);

        await MoveItemAsync(dbItem, shelfId, null, ItemMovementReason.Returned, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(dbItem);
    }


    public async Task<List<HistoryEntryResponse>> GetHistoryAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var dbItem = await _unitOfWork.Items.GetByIdAsync(itemId, cancellationToken);
        if (dbItem is null)
            throw new NotFoundException($"Item not found with {itemId}");

        var shelfRows = await _unitOfWork.ItemShelfHistories.Query()
            .Where(h => h.ItemId == itemId)
            .Select(h => new
            {
                h.ShelfId, h.PlacedAt, h.PlacedByUserId, h.Reason,
                h.RemovedAt, h.RemovedReason, h.RemovedByUserId
            })
            .ToListAsync(cancellationToken);

        var userRows = await _unitOfWork.ItemUserHistories.Query()
            .Where(h => h.ItemId == itemId)
            .Select(h => new
            {
                h.UserId, h.AssignedAt, h.AssignedByUserId, h.Reason,
                h.ReturnedAt, h.ReturnedReason, h.ReturnedByUserId
            })
            .ToListAsync(cancellationToken);

        var shelfEvents = shelfRows.SelectMany(h =>
        {
            var events = new List<HistoryEntryResponse>
            {
                new HistoryEntryResponse
                {
                    Action = h.Reason.ToString(), Type = "Shelf", Phase = "Opened",
                    ShelfId = h.ShelfId, UserId = null, ActorUserId = h.PlacedByUserId, OccurredAt = h.PlacedAt
                }
            };

            if (h.RemovedAt.HasValue)
            {
                events.Add(new HistoryEntryResponse
                {
                    Action = (h.RemovedReason ?? h.Reason).ToString(), Type = "Shelf", Phase = "Closed",
                    ShelfId = h.ShelfId, UserId = null, ActorUserId = h.RemovedByUserId, OccurredAt = h.RemovedAt.Value
                });
            }

            return events;
        });

        var userEvents = userRows.SelectMany(h =>
        {
            var events = new List<HistoryEntryResponse>
            {
                new HistoryEntryResponse
                {
                    Action = h.Reason.ToString(), Type = "User", Phase = "Opened",
                    ShelfId = null, UserId = h.UserId, ActorUserId = h.AssignedByUserId, OccurredAt = h.AssignedAt
                }
            };

            if (h.ReturnedAt.HasValue)
            {
                events.Add(new HistoryEntryResponse
                {
                    Action = (h.ReturnedReason ?? h.Reason).ToString(), Type = "User", Phase = "Closed",
                    ShelfId = null, UserId = h.UserId, ActorUserId = h.ReturnedByUserId, OccurredAt = h.ReturnedAt.Value
                });
            }

            return events;
        });

        return shelfEvents.Concat(userEvents).OrderByDescending(e => e.OccurredAt).ToList();
    }


    public async Task<List<MyActivityEntryResponse>> GetMyActivityAsync(CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();

        var shelfRows = await _unitOfWork.ItemShelfHistories.Query()
            .Where(h => h.PlacedByUserId == currentUserId || h.RemovedByUserId == currentUserId)
            .Select(h => new
            {
                h.ItemId, ItemName = h.Item.Name, h.ShelfId,
                h.PlacedAt, h.PlacedByUserId, h.Reason,
                h.RemovedAt, h.RemovedReason, h.RemovedByUserId
            })
            .ToListAsync(cancellationToken);

        var userRows = await _unitOfWork.ItemUserHistories.Query()
            .Where(h => h.AssignedByUserId == currentUserId || h.ReturnedByUserId == currentUserId)
            .Select(h => new
            {
                h.ItemId, ItemName = h.Item.Name,
                h.AssignedAt, h.AssignedByUserId, h.Reason,
                h.ReturnedAt, h.ReturnedReason, h.ReturnedByUserId
            })
            .ToListAsync(cancellationToken);

        var shelfEvents = shelfRows.SelectMany(h =>
        {
            var events = new List<MyActivityEntryResponse>();

            if (h.PlacedByUserId == currentUserId)
            {
                events.Add(new MyActivityEntryResponse
                {
                    ItemId = h.ItemId, ItemName = h.ItemName,
                    Action = h.Reason.ToString(), Type = "Shelf", Phase = "Opened",
                    ShelfId = h.ShelfId, OccurredAt = h.PlacedAt
                });
            }

            if (h.RemovedAt.HasValue && h.RemovedByUserId == currentUserId)
            {
                events.Add(new MyActivityEntryResponse
                {
                    ItemId = h.ItemId, ItemName = h.ItemName,
                    Action = (h.RemovedReason ?? h.Reason).ToString(), Type = "Shelf", Phase = "Closed",
                    ShelfId = h.ShelfId, OccurredAt = h.RemovedAt.Value
                });
            }

            return events;
        });

        var userEvents = userRows.SelectMany(h =>
        {
            var events = new List<MyActivityEntryResponse>();

            if (h.AssignedByUserId == currentUserId)
            {
                events.Add(new MyActivityEntryResponse
                {
                    ItemId = h.ItemId, ItemName = h.ItemName,
                    Action = h.Reason.ToString(), Type = "User", Phase = "Opened",
                    ShelfId = null, OccurredAt = h.AssignedAt
                });
            }

            if (h.ReturnedAt.HasValue && h.ReturnedByUserId == currentUserId)
            {
                events.Add(new MyActivityEntryResponse
                {
                    ItemId = h.ItemId, ItemName = h.ItemName,
                    Action = (h.ReturnedReason ?? h.Reason).ToString(), Type = "User", Phase = "Closed",
                    ShelfId = null, OccurredAt = h.ReturnedAt.Value
                });
            }

            return events;
        });

        return shelfEvents.Concat(userEvents).OrderByDescending(e => e.OccurredAt).ToList();
    }


    private async Task MoveItemAsync(Item dbItem, Guid? newShelfId, Guid? newUserId, ItemMovementReason reason, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var actorId = _currentUserService.GetCurrentUserId();

        if (newShelfId != dbItem.ShelfId)
        {
            if (dbItem.ShelfId.HasValue)
            {
                var oldShelf = await _unitOfWork.Shelves.GetByIdAsync(dbItem.ShelfId.Value, cancellationToken);
                if (oldShelf is not null)
                {
                    oldShelf.Capacity += 1;
                    oldShelf.UpdatedOn = now;
                }

                var openShelfHistory = await _unitOfWork.ItemShelfHistories.GetByItemId(dbItem.Id, cancellationToken);
                if (openShelfHistory is not null)
                {
                    openShelfHistory.RemovedAt = now;
                    openShelfHistory.RemovedReason = reason;
                    openShelfHistory.RemovedByUserId = actorId;
                    _unitOfWork.ItemShelfHistories.Update(openShelfHistory);
                }
            }

            if (newShelfId.HasValue)
            {
                var newShelf = await _unitOfWork.Shelves.GetByIdAsync(newShelfId.Value, cancellationToken);
                if (newShelf is null)
                    throw new NotFoundException($"Shelf not found with id : {newShelfId.Value}");

                if (newShelf.Capacity <= 0)
                    throw new NoMoreCapacityException($"No More capacity at shelf with id : {newShelfId.Value}. Current capacity : {newShelf.Capacity}");

                newShelf.Capacity -= 1;
                newShelf.UpdatedOn = now;

                
                var newHistory = new ItemShelfHistory()
                {
                    Id = Guid.NewGuid(),
                    ItemId = dbItem.Id,
                    ShelfId = newShelfId.Value,
                    PlacedAt = now,
                    PlacedByUserId = actorId,
                    Reason = reason
                };

                await _unitOfWork.ItemShelfHistories.AddAsync(newHistory, cancellationToken);
            }

            dbItem.ShelfId = newShelfId;
        }

        if (newUserId != dbItem.UserId)
        {
            if (dbItem.UserId.HasValue)
            {
                var openUserHistory = await _unitOfWork.ItemUserHistories.GetOpenByItemId(dbItem.Id, cancellationToken);
                
                if (openUserHistory is not null)
                {
                    openUserHistory.ReturnedAt = now;
                    openUserHistory.ReturnedReason = reason;
                    openUserHistory.ReturnedByUserId = actorId;
                
                    _unitOfWork.ItemUserHistories.Update(openUserHistory);
                }
            }

            if (newUserId.HasValue)
            {
                var newHistory = new ItemUserHistory()
                {
                    Id = Guid.NewGuid(),
                    ItemId = dbItem.Id,
                    UserId = newUserId.Value,
                    AssignedAt = now,
                    AssignedByUserId = actorId,
                    Reason = reason
                };

                await _unitOfWork.ItemUserHistories.AddAsync(newHistory, cancellationToken);
            }

            dbItem.UserId = newUserId;
        }

        dbItem.UpdatedOn = now;
    }

    private async Task ChangeStatusAsync(Item item, ItemStatus status, Guid actorId, CancellationToken cancellationToken)
    {
        item.Status = status;

        await _unitOfWork.ItemStatusHistories.AddAsync(new ItemStatusHistory
        {
            Id = Guid.NewGuid(),
            ItemId = item.Id,
            Status = status,
            UserId = actorId,
            ChangedAt = DateTime.UtcNow
        }, cancellationToken);
    }

}
