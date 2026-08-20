using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Dtos.Cabinet;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Controllers.Cabinet;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CabinetController : ControllerBase
{
    private readonly ICabinetService _cabinetService;

    public CabinetController(ICabinetService cabinetService)
    {
        _cabinetService = cabinetService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var result = await _cabinetService.CreateAsync(request,cancellationToken);
        return Ok(result);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> DeleteByIdAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var result = await _cabinetService.DeleteByIdAsync(cabinetId, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<Response>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _cabinetService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{cabinetId:guid}")]
    public async Task<ActionResult<Response>> GetByIdAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var result = await _cabinetService.GetCabinetByIdAsync(cabinetId, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await _cabinetService.UpdateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{cabinetId:guid}/shelves")]
    [ProducesResponseType(typeof(List<OfficeQr.Dtos.Shelf.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<OfficeQr.Dtos.Shelf.Response>>> GetShelvesAsync(Guid cabinetId, CancellationToken cancellationToken)
    {
        var result = await _cabinetService.GetShelvesAsync(cabinetId, cancellationToken);
        return Ok(result);
    }

} 