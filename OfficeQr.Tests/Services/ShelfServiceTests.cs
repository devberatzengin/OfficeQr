using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Shelf;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services;

namespace OfficeQr.Tests.Services;

public class ShelfServiceTests
{
    private static ShelfService CreateSut(Mock<IUnitOfWork> unitOfWorkMock)
    {
        var loggerMock = new Mock<ILogger<ShelfService>>();

        var createValidatorMock = new Mock<IValidator<CreateRequest>>();
        createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var updateValidatorMock = new Mock<IValidator<UpdateRequest>>();
        updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        return new ShelfService(
            unitOfWorkMock.Object,
            loggerMock.Object,
            createValidatorMock.Object,
            updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CabinetNotFound_ThrowsNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cabinet?)null);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock);

        var act = async () => await sut.CreateAsync(
            new CreateRequest { Capacity = 1, CabinetId = Guid.NewGuid() }, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CreateAsync_CabinetHasNoCapacity_ThrowsNoMoreCapacityException()
    {
        var dbCabinet = new Cabinet { Id = Guid.NewGuid(), Capacity = 0 };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(dbCabinet.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbCabinet);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock);

        var act = async () => await sut.CreateAsync(
            new CreateRequest { Capacity = 1, CabinetId = dbCabinet.Id }, CancellationToken.None);

        await Assert.ThrowsAsync<NoMoreCapacityException>(act);
    }

    [Fact]
    public async Task DeleteAsync_ShelfNotFound_ThrowsNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var shelfRepoMock = new Mock<IShelfRepository>();
        shelfRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shelf?)null);
        unitOfWorkMock.Setup(u => u.Shelves).Returns(shelfRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock);

        var act = async () => await sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}
