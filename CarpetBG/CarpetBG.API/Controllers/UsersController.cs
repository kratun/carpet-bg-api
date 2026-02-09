using System.Security.Claims;

using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("me")]
    [Authorize]
    public async Task<ActionResult<UserMeDto>> Me()
    {
        var email = User.FindFirst("https://api.yourapp.com/email")?.Value
        ?? User.FindFirst(ClaimTypes.Email)?.Value;
        if (email is null)
            return Unauthorized();

        var user = await userService.GetCurrentUserAsync(email ?? string.Empty);

        return user.IsSuccess ? Ok(user) : BadRequest(user);
    }
}
