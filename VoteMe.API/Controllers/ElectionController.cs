using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.Election;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/elections")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ElectionController : BaseController
{
    private readonly IElectionService _electionService;

    public ElectionController(IElectionService electionService)
    {
        _electionService = electionService;
    }

    [HttpPost]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> CreateElection([FromBody] CreateElectionDto dto)
    {
        var result = await _electionService.CreateElectionAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("{electionId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetElection(Guid electionId)
    {
        var result = await _electionService.GetElectionAsync(electionId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("organization/{organizationId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> GetOrganizationElections(Guid organizationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _electionService.GetOrganizationElectionsAsync(organizationId, page, pageSize);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPatch("{electionId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> UpdateElection(Guid electionId, [FromBody] UpdateElectionDto dto)
    {
        var result = await _electionService.UpdateElectionAsync(electionId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{electionId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> DeleteElection(Guid electionId)
    {
        var result = await _electionService.DeleteElectionAsync(electionId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("{electionId:guid}/results")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> GetElectionResults(Guid electionId)
    {
        var result = await _electionService.GetElectionResultsAsync(electionId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}