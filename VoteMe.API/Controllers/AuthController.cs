using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService , ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("change-password")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _authService.ChangePasswordAsync(_currentUserService.UserId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("logout")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync(_currentUserService.UserId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("register-organization")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterOrganization(
        [FromForm] CreateOrganizationDto dto)
    {
        var result = await _authService.RegisterOrganizationAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}