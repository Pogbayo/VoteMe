using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrganizationsController : BaseController
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet("{organizationId:guid}")]
    [Authorize(Policy = "Authenticated")] 
    public async Task<IActionResult> GetOrganization(Guid organizationId)
    {
        var result = await _organizationService.GetOrganizationAsync(organizationId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPatch("{organizationId:guid}")]
    [Authorize(Policy = "OrgAdminOrSuperAdmin")]
    public async Task<IActionResult> UpdateOrganization([FromRoute]Guid organizationId, [FromForm] UpdateOrganizationDto dto)
    {
        var result = await _organizationService.UpdateOrganizationAsync(organizationId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{organizationId:guid}")]
    [Authorize(Policy = "OrgAdminOrSuperAdmin")]
    public async Task<IActionResult> DeleteOrganization(Guid organizationId)
    {
        var result = await _organizationService.DeleteOrganizationAsync(organizationId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}