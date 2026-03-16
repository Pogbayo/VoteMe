using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrganizationMemberController : BaseController
{
    private readonly IOrganizationMemberService _organizationMemberService;

    public OrganizationMemberController(IOrganizationMemberService organizationMemberService)
    {
        _organizationMemberService = organizationMemberService;
    }

    [HttpPost("join")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> JoinOrganization([FromBody] string uniqueKey)
    {
        var result = await _organizationMemberService.JoinOrganizationAsync(uniqueKey);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{organizationId:guid}/members/{userId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> RemoveMember(Guid organizationId, Guid userId)
    {
        var result = await _organizationMemberService.RemoveMemberAsync(organizationId, userId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("{organizationId:guid}/promote/{userId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> PromoteToAdmin(Guid organizationId, Guid userId)
    {
        var result = await _organizationMemberService.PromoteToAdminAsync(organizationId, userId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("{organizationId:guid}/demote/{userId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> DemoteFromAdmin(Guid organizationId, Guid userId)
    {
        var result = await _organizationMemberService.DemoteFromAdminAsync(organizationId, userId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost("{organizationId:guid}/leave")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> LeaveOrganization(Guid organizationId)
    {
        var result = await _organizationMemberService.LeaveOrganizationAsync(organizationId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("my-organizations")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetUserOrganizations()
    {
        var result = await _organizationMemberService.GetUserOrganizationsAsync();
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("{organizationId:guid}/members")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> GetMembers(Guid organizationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _organizationMemberService.GetMembersAsync(organizationId, page, pageSize);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}