using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.User;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var result = await _userService.GetUserAsync(userId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPatch("{userId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateUserAsync(userId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{userId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _userService.DeleteUserAsync(userId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("organization/{organizationId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> GetAllOrganizationUsers(Guid organizationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.GetAllUsersAsync(organizationId, page, pageSize);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}