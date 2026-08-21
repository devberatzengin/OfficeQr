using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Dtos.User;
using OfficeQr.Services.Interfaces;

namespace OfficeQr.Controllers.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> DeactivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.DeactivateAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId:guid}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> ActivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.ActivateAsync(userId, cancellationToken);
        return Ok(result);
    }
}
