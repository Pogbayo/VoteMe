using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CandidateController : BaseController
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet("{candidateId:guid}")]
    public async Task<IActionResult> GetCandidate(Guid candidateId)
    {
        var result = await _candidateService.GetCandidateAsync(candidateId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> AddCandidate([FromForm] CreateCandidateDto dto)
    {
        var result = await _candidateService.AddCandidateAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPatch("{candidateId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> UpdateCandidate([FromRoute]Guid candidateId, [FromForm] UpdateCandidateDto dto)
    {
        var result = await _candidateService.UpdateCandidateAsync(candidateId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{candidateId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> DeleteCandidate(Guid candidateId)
    {
        var result = await _candidateService.DeleteCandidateAsync(candidateId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("category/{electionCategoryId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetCategoryCandidates(Guid electionCategoryId)
    {
        var result = await _candidateService.GetCategoryCandidatesAsync(electionCategoryId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}