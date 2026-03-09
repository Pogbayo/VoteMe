using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Election;
using VoteMe.Application.DTOs.Vote;

namespace VoteMe.Application.Interface.IServices
{
    public interface IVoteService
    {
        Task<ApiResponse<VoteResultDto>> CastVoteAsync(Guid userId, CastVoteDto dto);
        Task<ApiResponse<VoteResultDto>> ChangeVoteAsync(Guid userId, ChangeVoteDto dto);
        Task<ApiResponse<bool>> HasUserVotedAsync(Guid userId, Guid electionId);
        Task<ApiResponse<ElectionResultDto>> GetLiveResultsAsync(Guid electionId);
        Task<ApiResponse<ElectionResultDto>> GetElectionResultsAsync(Guid electionId);
    }
}
