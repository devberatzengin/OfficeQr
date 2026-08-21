using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Item;
using OfficeQr.Entity;
using OfficeQr.Entity.Enums;
using OfficeQr.Exceptions;
using OfficeQr.Services;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Tests.Services;

public class ItemServiceTests
{
    private class ItemServiceFixture
    {
        public Mock<IMapper> MapperMock { get; } = new();
        public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
        public Mock<IItemRepository> ItemRepoMock { get; } = new();
        public Mock<IShelfRepository> ShelfRepoMock { get; } = new();
        public Mock<IItemShelfHistoryRepository> ItemShelfHistoryRepoMock { get; } = new();
        public Mock<IItemUserHistoryRepository> ItemUserHistoryRepoMock { get; } = new();
        public Mock<IItemStatusHistoryRepository> ItemStatusHistoryRepoMock { get; } = new();
        public Mock<ICurrentUserService> CurrentUserServiceMock { get; } = new();

        public ItemServiceFixture()
        {
            UnitOfWorkMock.Setup(u => u.Items).Returns(ItemRepoMock.Object);
            UnitOfWorkMock.Setup(u => u.Shelves).Returns(ShelfRepoMock.Object);
            UnitOfWorkMock.Setup(u => u.ItemShelfHistories).Returns(ItemShelfHistoryRepoMock.Object);
            UnitOfWorkMock.Setup(u => u.ItemUserHistories).Returns(ItemUserHistoryRepoMock.Object);
            UnitOfWorkMock.Setup(u => u.ItemStatusHistories).Returns(ItemStatusHistoryRepoMock.Object);
            UnitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        public ItemService CreateSut()
        {
            var createValidatorMock = new Mock<IValidator<CreateRequest>>();
            createValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<CreateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var returnValidatorMock = new Mock<IValidator<ReturnRequest>>();
            returnValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ReturnRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var updateValidatorMock = new Mock<IValidator<UpdateRequest>>();
            updateValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            return new ItemService(
                MapperMock.Object,
                UnitOfWorkMock.Object,
                new Mock<ILogger<ItemService>>().Object,
                returnValidatorMock.Object,
                createValidatorMock.Object,
                updateValidatorMock.Object,
                CurrentUserServiceMock.Object);
        }
    }

    [Fact]
    public async Task CreateAsync_ShelfNotFound_ThrowsNotFoundException()
    {
        var fx = new ItemServiceFixture();
        fx.ShelfRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shelf?)null);

        var sut = fx.CreateSut();

        var act = async () => await sut.CreateAsync(
            new CreateRequest { Name = "Mouse", ShelfId = Guid.NewGuid() }, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CreateAsync_ShelfHasNoCapacity_ThrowsNoMoreCapacityException()
    {
        var fx = new ItemServiceFixture();
        var dbShelf = new Shelf { Id = Guid.NewGuid(), Capacity = 0 };
        fx.ShelfRepoMock
            .Setup(r => r.GetByIdAsync(dbShelf.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbShelf);

        var sut = fx.CreateSut();

        var act = async () => await sut.CreateAsync(
            new CreateRequest { Name = "Mouse", ShelfId = dbShelf.Id }, CancellationToken.None);

        await Assert.ThrowsAsync<NoMoreCapacityException>(act);
        fx.ItemRepoMock.Verify(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsItemAndRecordsShelfAndStatusHistory()
    {
        var fx = new ItemServiceFixture();
        var dbShelf = new Shelf { Id = Guid.NewGuid(), Capacity = 3 };
        var actorId = Guid.NewGuid();

        fx.ShelfRepoMock
            .Setup(r => r.GetByIdAsync(dbShelf.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbShelf);
        fx.CurrentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(actorId);

        var sut = fx.CreateSut();

        await sut.CreateAsync(new CreateRequest { Name = "Mouse", ShelfId = dbShelf.Id }, CancellationToken.None);

        Assert.Equal(2, dbShelf.Capacity);
        fx.ItemRepoMock.Verify(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        fx.ItemShelfHistoryRepoMock.Verify(
            r => r.AddAsync(It.Is<ItemShelfHistory>(h => h.ShelfId == dbShelf.Id && h.Reason == ItemMovementReason.Created), It.IsAny<CancellationToken>()),
            Times.Once);
        fx.ItemStatusHistoryRepoMock.Verify(
            r => r.AddAsync(It.Is<ItemStatusHistory>(h => h.Status == ItemStatus.Available), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(ItemStatus.InUse)]
    [InlineData(ItemStatus.Maintenance)]
    public async Task PickupAsync_ItemNotAvailable_ThrowsBadRequestException(ItemStatus status)
    {
        var fx = new ItemServiceFixture();
        var dbItem = new Item { Id = Guid.NewGuid(), Status = status };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);

        var sut = fx.CreateSut();

        var act = async () => await sut.PickupAsync(dbItem.Id, CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(act);
        fx.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PickupAsync_ItemAvailable_SetsStatusToInUseAndRecordsUserHistory()
    {
        var fx = new ItemServiceFixture();
        var shelfId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dbItem = new Item { Id = Guid.NewGuid(), Status = ItemStatus.Available, ShelfId = shelfId, UserId = null };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);
        fx.CurrentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);

        var sut = fx.CreateSut();

        await sut.PickupAsync(dbItem.Id, CancellationToken.None);

        Assert.Equal(ItemStatus.InUse, dbItem.Status);
        Assert.Equal(userId, dbItem.UserId);
        fx.ItemUserHistoryRepoMock.Verify(
            r => r.AddAsync(It.Is<ItemUserHistory>(h => h.UserId == userId && h.Reason == ItemMovementReason.PickedUp), It.IsAny<CancellationToken>()),
            Times.Once);
        fx.ItemStatusHistoryRepoMock.Verify(
            r => r.AddAsync(It.Is<ItemStatusHistory>(h => h.Status == ItemStatus.InUse), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReturnAsync_ItemNotInUse_ThrowsBadRequestException()
    {
        var fx = new ItemServiceFixture();
        var dbItem = new Item { Id = Guid.NewGuid(), Status = ItemStatus.Available };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);

        var sut = fx.CreateSut();

        var act = async () => await sut.ReturnAsync(dbItem.Id, Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(act);
        fx.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReturnAsync_ItemInUse_SetsStatusToAvailableAndClosesUserHistory()
    {
        var fx = new ItemServiceFixture();
        var shelfId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dbItem = new Item { Id = Guid.NewGuid(), Status = ItemStatus.InUse, ShelfId = shelfId, UserId = userId };

        var openUserHistory = new ItemUserHistory
        {
            Id = Guid.NewGuid(),
            ItemId = dbItem.Id,
            UserId = userId,
            AssignedAt = DateTime.UtcNow.AddHours(-1),
            Reason = ItemMovementReason.PickedUp
        };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);
        fx.ItemUserHistoryRepoMock
            .Setup(r => r.GetOpenByItemId(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(openUserHistory);

        var sut = fx.CreateSut();

        await sut.ReturnAsync(dbItem.Id, shelfId, CancellationToken.None);

        Assert.Equal(ItemStatus.Available, dbItem.Status);
        Assert.Null(dbItem.UserId);
        Assert.NotNull(openUserHistory.ReturnedAt);
        fx.ItemUserHistoryRepoMock.Verify(r => r.Update(openUserHistory), Times.Once);
    }

    [Fact]
    public async Task ItemToShelf_NewShelfHasNoCapacity_ThrowsNoMoreCapacityException()
    {
        var fx = new ItemServiceFixture();
        var oldShelfId = Guid.NewGuid();
        var newShelfId = Guid.NewGuid();
        var dbItem = new Item { Id = Guid.NewGuid(), ShelfId = oldShelfId };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);
        fx.ShelfRepoMock
            .Setup(r => r.GetByIdAsync(oldShelfId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Shelf { Id = oldShelfId, Capacity = 2 });
        fx.ShelfRepoMock
            .Setup(r => r.GetByIdAsync(newShelfId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Shelf { Id = newShelfId, Capacity = 0 });

        var sut = fx.CreateSut();

        var act = async () => await sut.ItemToShelf(dbItem.Id, newShelfId, null, CancellationToken.None);

        await Assert.ThrowsAsync<NoMoreCapacityException>(act);
    }

    [Fact]
    public async Task ItemToShelf_MovesItem_UpdatesShelfCapacitiesAndClosesOldHistory()
    {
        var fx = new ItemServiceFixture();
        var oldShelfId = Guid.NewGuid();
        var newShelfId = Guid.NewGuid();
        var dbItem = new Item { Id = Guid.NewGuid(), ShelfId = oldShelfId };

        var oldShelf = new Shelf { Id = oldShelfId, Capacity = 2 };
        var newShelf = new Shelf { Id = newShelfId, Capacity = 3 };

        var openShelfHistory = new ItemShelfHistory
        {
            Id = Guid.NewGuid(),
            ItemId = dbItem.Id,
            ShelfId = oldShelfId,
            PlacedAt = DateTime.UtcNow.AddDays(-1),
            Reason = ItemMovementReason.Created
        };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);
        fx.ShelfRepoMock.Setup(r => r.GetByIdAsync(oldShelfId, It.IsAny<CancellationToken>())).ReturnsAsync(oldShelf);
        fx.ShelfRepoMock.Setup(r => r.GetByIdAsync(newShelfId, It.IsAny<CancellationToken>())).ReturnsAsync(newShelf);
        fx.ItemShelfHistoryRepoMock
            .Setup(r => r.GetByItemId(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(openShelfHistory);

        var sut = fx.CreateSut();

        await sut.ItemToShelf(dbItem.Id, newShelfId, null, CancellationToken.None);

        Assert.Equal(3, oldShelf.Capacity);
        Assert.Equal(2, newShelf.Capacity);
        Assert.Equal(newShelfId, dbItem.ShelfId);
        Assert.NotNull(openShelfHistory.RemovedAt);
        fx.ItemShelfHistoryRepoMock.Verify(r => r.Update(openShelfHistory), Times.Once);
        fx.ItemShelfHistoryRepoMock.Verify(
            r => r.AddAsync(It.Is<ItemShelfHistory>(h => h.ShelfId == newShelfId && h.Reason == ItemMovementReason.Moved), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PickupAsync_ConcurrentConflict_PropagatesConcurrencyConflictException()
    {
        var fx = new ItemServiceFixture();
        var dbItem = new Item { Id = Guid.NewGuid(), Status = ItemStatus.Available };

        fx.ItemRepoMock
            .Setup(r => r.GetByIdAsync(dbItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbItem);
        fx.UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyConflictException("Bu ürün senden hemen önce başka biri tarafından değiştirildi."));

        var sut = fx.CreateSut();

        var act = async () => await sut.PickupAsync(dbItem.Id, CancellationToken.None);

        await Assert.ThrowsAsync<ConcurrencyConflictException>(act);
    }
}
