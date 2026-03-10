using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CandidateService> _logger;
        private readonly IMessageBus _messageBus;

        public CandidateService(IUnitOfWork unitOfWork, ILogger<CandidateService> logger, IMessageBus messageBus)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _messageBus = messageBus;
        }

        public Task<ApiResponse<CandidateDto>> AddCandidateAsync(Guid electionId, CreateCandidateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteCandidateAsync(Guid candidateId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CandidateDto>> GetCandidateAsync(Guid candidateId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<CandidateDto>>> GetCategoryCandidatesAsync(Guid electionCategoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CandidateDto>> UpdateCandidateAsync(Guid candidateId, UpdateCandidateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
