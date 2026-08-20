using System.Text.Json;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Cabinet;
using OfficeQr.Entity;
using OfficeQr.Exceptions;
using OfficeQr.Helpers;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Services;


public class CabinetService : ICabinetService
{

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CabinetService> _logger;

    private readonly IValidator<CreateRequest> _createRequestValidator;
    private readonly IValidator<UpdateRequest> _updateRequestValidator;

    public CabinetService(
        IMapper mapper,
        IUnitOfWork unitOfWork, 
        ILogger<CabinetService> logger,
        IValidator<CreateRequest> createRequestValidator,
        IValidator<UpdateRequest> updateRequestValidator)
    {
        _mapper = mapper;
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
        

        Cabinet newCabinet = new Cabinet()
        {
            Id = Guid.NewGuid(),
            QrCode = "",
            Capacity = request.Capacity,
            Shelves = new List<Shelf>(),
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        QrCodeGenerator qrCodeGenerator = new QrCodeGenerator();
        newCabinet.QrCode = "data:image/png;base64," + qrCodeGenerator.CreateQr(newCabinet);

        await _unitOfWork.Cabinets.AddAsync(newCabinet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(newCabinet);


    }

    public async Task<bool> DeleteByIdAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var dbResult = await _unitOfWork.Cabinets.GetByIdAsync(cabinetId, cancellationToken);
        if (dbResult is null)
        {
            throw new NotFoundException($"Cabinet not found with {cabinetId} id");
        }

        dbResult.IsDeleted = true;
        dbResult.UpdatedOn = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;

    }

    public async Task<List<Response>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cabinets = await _unitOfWork.Cabinets.GetAllAsync(cancellationToken);

        if (cabinets is null)
        {
            throw new NotFoundException("Not add any cabinets add yet");
        }

        
        var listedCabinet = cabinets.Select(item => new Response {
            Id = item.Id,
            Capacity = item.Capacity
        }).ToList();
        
        return _mapper.Map<List<Response>>(cabinets);


    }

    public async Task<Response> GetCabinetByIdAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var dbCabinet = await _unitOfWork.Cabinets.GetByIdAsync(cabinetId, cancellationToken);
        if ( dbCabinet is null)
            throw new NotFoundException($"Cabinet not found with {cabinetId} id");
        
        return _mapper.Map<Response>(dbCabinet);
    }



    public async Task<Response> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
    {

        var validationResult = await _updateRequestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException($"Request is not valid: {JsonSerializer.Serialize(validationResult.ToDictionary())}");
        

        var dbCabinet = await _unitOfWork.Cabinets.GetByIdAsync(request.Id, cancellationToken);

        if (dbCabinet is null)
        {
            throw new NotFoundException($"Cabinet not found with {request.Id} id");
        }

        if (request.Capacity.HasValue)
        {
            dbCabinet.Capacity = request.Capacity.Value;
        }

        dbCabinet.UpdatedOn = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Response>(dbCabinet);
    }


    public async Task<List<Dtos.Shelf.Response>> GetShelvesAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var dbCabinet = await _unitOfWork.Cabinets.GetByIdAsync(cabinetId, cancellationToken);
        if (dbCabinet is null)
            throw new NotFoundException($"Cabinet not found with {cabinetId} id");

        var shelves = await _unitOfWork.Shelves.Query()
            .Where(s => s.CabinetId == cabinetId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<Dtos.Shelf.Response>>(shelves);
    }

}