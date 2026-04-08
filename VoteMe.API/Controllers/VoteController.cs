using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VoteMe.Application.DTOs.Vote;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Controllers;

[ApiController]
[Route("api/votes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VotesController : BaseController
{
    private readonly IVoteService _voteService;

    public VotesController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPost("{candidateId:guid}")]
    [EnableRateLimiting("voting")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> CastVote([FromRoute] Guid candidateId)
    {
        var result = await _voteService.CastVoteAsync(candidateId);
        return result.Success ? OkResponse(true, "Vote cast successfully") : ErrorResponse("Vote failed");
    }

    [HttpGet("{organizationId:guid}/total-votes")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetOrganizationVotesCount([FromRoute] Guid organizationId)
    {
        var result = await _voteService.GetOrganizationVotesCount(organizationId);
        return result.Success ? OkResponse(result.
            Data, "Vote count") : ErrorResponse("Vote failed");
    }
}