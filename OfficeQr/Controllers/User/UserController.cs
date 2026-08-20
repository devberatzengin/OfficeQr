using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeQr.Dtos.User;
using OfficeQr.Entity;

namespace OfficeQr.Controllers.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<OfficeQr.Entity.User> _userManager;

    public UsersController(UserManager<OfficeQr.Entity.User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return NotFound();

        return Ok(new UserResponse { Id = user.Id, Email = user.Email ?? string.Empty });
    }
}