using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Data.Interfaces;
using OfficeQr.Dtos.Shelf;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Controllers.Shelf;



[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShelfController : ControllerBase
{

    private readonly IShelfService _shelfService;

    public ShelfController (IShelfService shelfService)
    {
        _shelfService = shelfService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateAsync (CreateRequest request, CancellationToken cancellationToken)
    {
        var result = await _shelfService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shelfId}")]
    public async Task<ActionResult<Response>> GetByIdAsync(Guid shelfId, CancellationToken cancellationToken)
    {
        var result = await _shelfService.GetByIdAsync(shelfId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{shelfId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> DeleteAsync(Guid shelfId, CancellationToken cancellationToken)
    {
        var result = await _shelfService.DeleteAsync(shelfId, cancellationToken);
        return Ok(result);
    }


    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await _shelfService.UpdateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{shelfId}/items")]
    public async Task<ActionResult<List<OfficeQr.Dtos.Item.Response>>> GetShelfItemsById(Guid shelfId, CancellationToken cancellationToken)
    {
        var result = await _shelfService.GetShelfItemsById(shelfId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{shelfId:guid}/cabinet")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> MoveToCabinetAsync(Guid shelfId, MoveRequest request, CancellationToken cancellationToken)
    {
        var result = await _shelfService.MoveToCabinetAsync(shelfId, request.CabinetId, cancellationToken);
        return Ok(result);
    }

}
