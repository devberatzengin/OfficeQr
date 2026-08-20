using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Dtos.Common;
using OfficeQr.Dtos.Item;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Controllers.Item;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController (IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var result = await _itemService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<Response>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<Response>>> GetAllAsync([FromQuery] ListRequest request,CancellationToken cancellationToken)
    {
        var result = await _itemService.GetAllAsync(request,cancellationToken);
        return Ok(result);
    }

    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Response>> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetItemByIdAsync(itemId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{itemId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> DeleteAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _itemService.DeleteByIdAsync(itemId, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateAsync(UpdateRequest request,CancellationToken cancellationToken)
    {
        var result = await _itemService.UpdateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{itemId:guid}/shelf")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> MoveAsync(Guid itemId, MoveRequest request, CancellationToken cancellationToken)
    {
        var result = await _itemService.ItemToShelf(itemId, request.ShelfId, request.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{itemId:guid}/pickup")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    
    public async Task<ActionResult<Response>> PickupAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _itemService.PickupAsync(itemId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{itemId:guid}/return")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Response>> ReturnAsync(Guid itemId, ReturnRequest request, CancellationToken cancellationToken)
    {
        var result = await _itemService.ReturnAsync(itemId, request.ShelfId, cancellationToken);
        return Ok(result);
    }


    [HttpGet("{itemId:guid}/history")]
    [ProducesResponseType(typeof(List<HistoryEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HistoryEntryResponse>>> GetHistoryAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetHistoryAsync(itemId, cancellationToken);
        return Ok(result);
    }


    [HttpGet("my-activity")]
    [ProducesResponseType(typeof(List<MyActivityEntryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MyActivityEntryResponse>>> GetMyActivityAsync(CancellationToken cancellationToken)
    {
        var result = await _itemService.GetMyActivityAsync(cancellationToken);
        return Ok(result);
    }
}


