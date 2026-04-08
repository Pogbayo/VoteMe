using VoteMe.Application.Common;

namespace VoteMe.Application.Interface.IServices
{
    public interface IVoteService
    {
        Task<ApiResponse<bool>> CastVoteAsync(Guid candidateId);
        Task<ApiResponse<int>> GetOrganizationVotesCount(Guid organizationId);
        //Task<ApiResponse<bool>> HasUserVotedAsync(Guid userId, Guid electionId);
        //Task<ApiResponse<ElectionResultDto>> GetLiveResultsAsync(Guid electionId);
        //Task<ApiResponse<ElectionResultDto>> GetElectionResultsAsync(Guid electionId);
    }
}
