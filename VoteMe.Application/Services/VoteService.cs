
using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Election;
using VoteMe.Application.DTOs.Vote;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class VoteService : IVoteService
    {
        public Task<ApiResponse<VoteResultDto>> CastVoteAsync(Guid userId, CastVoteDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<VoteResultDto>> ChangeVoteAsync(Guid userId, ChangeVoteDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionResultDto>> GetLiveResultsAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> HasUserVotedAsync(Guid userId, Guid electionId)
        {
            throw new NotImplementedException();
        }
    }
}
