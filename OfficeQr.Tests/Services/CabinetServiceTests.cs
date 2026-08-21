using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Cabinet;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Services;

namespace OfficeQr.Tests.Services;

public class CabinetServiceTests
{
    private static CabinetService CreateSut(
        Mock<IUnitOfWork> unitOfWorkMock,
        Mock<IMapper>? mapperMock = null,
        bool createRequestValid = true,
        bool updateRequestValid = true)
    {
        mapperMock ??= new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<CabinetService>>();

        var createValidatorMock = new Mock<IValidator<CreateRequest>>();
        createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createRequestValid
                ? new ValidationResult()
                : new ValidationResult(new List<ValidationFailure> { new("Capacity", "Invalid capacity") }));

        var updateValidatorMock = new Mock<IValidator<UpdateRequest>>();
        updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateRequestValid
                ? new ValidationResult()
                : new ValidationResult(new List<ValidationFailure> { new("Capacity", "Invalid capacity") }));

        return new CabinetService(
            mapperMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object,
            createValidatorMock.Object,
            updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsValidationException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock, createRequestValid: false);

        var act = async () => await sut.CreateAsync(new CreateRequest { Capacity = 5 }, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
        cabinetRepoMock.Verify(r => r.AddAsync(It.IsAny<Cabinet>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsCabinetAndSaves()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<Response>(It.IsAny<Cabinet>())).Returns(new Response { Capacity = 5 });

        var sut = CreateSut(unitOfWorkMock, mapperMock);

        var result = await sut.CreateAsync(new CreateRequest { Capacity = 5 }, CancellationToken.None);

        Assert.Equal(5, result.Capacity);
        cabinetRepoMock.Verify(r => r.AddAsync(It.IsAny<Cabinet>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCabinetByIdAsync_CabinetNotFound_ThrowsNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cabinet?)null);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock);

        var act = async () => await sut.GetCabinetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetCabinetByIdAsync_CabinetExists_ReturnsResponse()
    {
        var cabinetId = Guid.NewGuid();
        var dbCabinet = new Cabinet { Id = cabinetId, Capacity = 7 };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(cabinetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbCabinet);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<Response>(dbCabinet)).Returns(new Response { Id = cabinetId, Capacity = 7 });

        var sut = CreateSut(unitOfWorkMock, mapperMock);

        var result = await sut.GetCabinetByIdAsync(cabinetId, CancellationToken.None);

        Assert.Equal(cabinetId, result.Id);
        Assert.Equal(7, result.Capacity);
    }

    [Fact]
    public async Task DeleteByIdAsync_CabinetNotFound_ThrowsNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cabinet?)null);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock);

        var act = async () => await sut.DeleteByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task DeleteByIdAsync_CabinetExists_SetsIsDeletedAndReturnsTrue()
    {
        var dbCabinet = new Cabinet { Id = Guid.NewGuid(), IsDeleted = false };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        cabinetRepoMock
            .Setup(r => r.GetByIdAsync(dbCabinet.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbCabinet);
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut(unitOfWorkMock);

        var result = await sut.DeleteByIdAsync(dbCabinet.Id, CancellationToken.None);

        Assert.True(result);
        Assert.True(dbCabinet.IsDeleted);
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsValidationException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var cabinetRepoMock = new Mock<ICabinetRepository>();
        unitOfWorkMock.Setup(u => u.Cabinets).Returns(cabinetRepoMock.Object);

        var sut = CreateSut(unitOfWorkMock, updateRequestValid: false);

        var act = async () => await sut.UpdateAsync(
            new UpdateRequest { Id = Guid.NewGuid(), Capacity = 5 }, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
    }
}
