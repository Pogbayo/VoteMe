using Microsoft.AspNetCore.Identity;
using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly INotificationService _notificationService;
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            INotificationService notificationService,
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _messageBus = messageBus;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> LogoutAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
