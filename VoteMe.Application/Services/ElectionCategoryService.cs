using VoteMe.Application.Common;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class ElectionCategoryService : IElectionCategoryService
    {
        public Task<ApiResponse<ElectionCategoryDto>> CreateCategoryAsync(Guid electionId, CreateElectionCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteCategoryAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionCategoryDto>> GetCategoryAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionCategoryResultDto>> GetCategoryResultsAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<ElectionCategoryDto>>> GetElectionCategoriesAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionCategoryDto>> UpdateCategoryAsync(Guid categoryId, UpdateElectionCategoryDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
