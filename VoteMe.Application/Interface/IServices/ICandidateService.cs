using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.Interface.IServices
{
    public interface ICandidateService
    {
        Task<ApiResponse<CandidateDto>> GetCandidateAsync(Guid candidateId);
        Task<ApiResponse<CandidateDto>> AddCandidateAsync(CreateCandidateDto dto);
        Task<ApiResponse<CandidateDto>> UpdateCandidateAsync(Guid candidateId, UpdateCandidateDto dto);
        Task<ApiResponse<bool>> DeleteCandidateAsync(Guid candidateId);
        Task<ApiResponse<IEnumerable<CandidateDto>>> GetCategoryCandidatesAsync(Guid electionCategoryId);
    }
}
