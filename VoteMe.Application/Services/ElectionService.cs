using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Election;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
     public class ElectionService : IElectionService
     {
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ApiResponse<ElectionDto>> CloseElectionAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionDto>> CreateElectionAsync(Guid organizationId, CreateElectionDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteElectionAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionDto>> GetElectionAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionResultDto>> GetElectionResultsAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<(IEnumerable<ElectionDto> Items, int TotalCount)>> GetOrganizationElectionsAsync(
             Guid organizationId,
             int page = 1,
             int pageSize = 20)
        {
            // GetPagedAsync lives in GenericRepository
            // ElectionRepository inherits it
            // _unitOfWork.Elections points to ElectionRepository
            // which has _dbSet pointing at Elections table
            var (elections, totalCount) = await _unitOfWork.Elections.GetPagedAsync(
                predicate: e => e.OrganizationId == organizationId,
                page: page,
                pageSize: pageSize
            );

            var electionDtos = elections.Select(e => new ElectionDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status.ToString(),
                IsPrivate = e.IsPrivate,
                OrganizationId = e.OrganizationId,
                CreatedAt = e.CreatedAt
            });

            return ApiResponse<(IEnumerable<ElectionDto>, int)>.SuccessResponse(
                (electionDtos, totalCount),
                "Elections retrieved successfully"
            );
        }


        public Task<ApiResponse<ElectionDto>> OpenElectionAsync(Guid electionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ElectionDto>> UpdateElectionAsync(Guid electionId, UpdateElectionDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
