using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ElectionCategoryController : BaseController
{
    private readonly IElectionCategoryService _electionCategoryService;

    public ElectionCategoryController(IElectionCategoryService electionCategoryService)
    {
        _electionCategoryService = electionCategoryService;
    }

    [HttpGet("{electionCategoryId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetElectionCategory(Guid electionCategoryId)
    {
        var result = await _electionCategoryService.GetElectionCategoryAsync(electionCategoryId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPost]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> CreateElectionCategory([FromBody] CreateElectionCategoryDto dto)
    {
        var result = await _electionCategoryService.CreateElectionCategoryAsync(dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpPatch("{electionCategoryId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> UpdateElectionCategory([FromRoute] Guid electionCategoryId, [FromBody] UpdateElectionCategoryDto dto)
    {
        var result = await _electionCategoryService.UpdateElectionCategoryAsync(electionCategoryId, dto);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpDelete("{electionCategoryId:guid}")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> DeleteElectionCategory(Guid electionCategoryId)
    {
        var result = await _electionCategoryService.DeleteElectionCategoryAsync(electionCategoryId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("election/{electionId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetElectionCategories(Guid electionId)
    {
        var result = await _electionCategoryService.GetElectionCategoriesAsync(electionId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }

    [HttpGet("{electionCategoryId:guid}/results")]
    [Authorize(Policy = "OrgAdmin")]
    public async Task<IActionResult> GetElectionCategoryResults(Guid electionCategoryId)
    {
        var result = await _electionCategoryService.GetElectionCategoryResultsAsync(electionCategoryId);
        return result.Success ? OkResponse(result.Data, result.Message) : ErrorResponse(result.Message, result.Errors);
    }
}