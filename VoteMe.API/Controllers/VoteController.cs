using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    [Authorize(Policy = "Voter")]
    public async Task<IActionResult> CastVote([FromBody] CastVoteDto dto)
    {
        var result = await _voteService.CastVoteAsync(dto);
        return result.Success ? OkResponse(true, "Vote cast successfully") : ErrorResponse("Vote failed");
    }
}