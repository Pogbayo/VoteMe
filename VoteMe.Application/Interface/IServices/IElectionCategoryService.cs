using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.Interface.IServices
{
    public interface IElectionCategoryService
    {
        Task<ApiResponse<ElectionCategoryDto>> GetCategoryAsync(Guid categoryId);
        Task<ApiResponse<ElectionCategoryDto>> CreateCategoryAsync(Guid electionId, CreateElectionCategoryDto dto);
        Task<ApiResponse<ElectionCategoryDto>> UpdateCategoryAsync(Guid categoryId, UpdateElectionCategoryDto dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid categoryId);
        Task<ApiResponse<IEnumerable<ElectionCategoryDto>>> GetElectionCategoriesAsync(Guid electionId);
        Task<ApiResponse<ElectionCategoryResultDto>> GetCategoryResultsAsync(Guid categoryId);
    }
}
