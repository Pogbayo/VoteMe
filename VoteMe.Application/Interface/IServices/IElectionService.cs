using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Election;

namespace VoteMe.Application.Interface.IServices
{
    public interface IElectionService
    {
        Task<ApiResponse<ElectionDto>> CreateElectionAsync(CreateElectionDto dto);
        Task<ApiResponse<ElectionDto>> GetElectionAsync(Guid electionId);
        Task<ApiResponse<PagedElectionResponse>> GetOrganizationElectionsAsync(Guid organizationId, int page = 1, int pageSize = 20);
        Task<ApiResponse<ElectionDto>> UpdateElectionAsync(Guid electionId, UpdateElectionDto dto);
        Task<ApiResponse<bool>> DeleteElectionAsync(Guid electionId);
        Task<ApiResponse<ElectionDto>> OpenElectionAsync(Guid electionId);
        Task<ApiResponse<ElectionResultDto>> GetElectionResultsAsync(Guid electionId);
    }
}
