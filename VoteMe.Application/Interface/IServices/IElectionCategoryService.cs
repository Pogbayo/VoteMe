using VoteMe.Application.Common;
using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.Interface.IServices;

public interface IElectionCategoryService
{
    Task<ApiResponse<ElectionCategoryDto>> GetElectionCategoryAsync(Guid electionCategoryId);

    Task<ApiResponse<ElectionCategoryDto>> CreateElectionCategoryAsync(
        Guid electionId,
        CreateElectionCategoryDto dto);

    Task<ApiResponse<ElectionCategoryDto>> UpdateElectionCategoryAsync(
        Guid electionCategoryId,
        UpdateElectionCategoryDto dto);

    Task<ApiResponse<bool>> DeleteElectionCategoryAsync(Guid electionCategoryId);

    Task<ApiResponse<IEnumerable<ElectionCategoryDto>>> GetElectionCategoriesAsync(Guid electionId);

    Task<ApiResponse<ElectionCategoryResultDto>> GetElectionCategoryResultsAsync(Guid electionCategoryId);
}