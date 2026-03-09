using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Auth;

namespace VoteMe.Application.Interface.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<ApiResponse<bool>> LogoutAsync(Guid userId);
    }
}
