using Microsoft.AspNetCore.Http;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;

namespace VoteMe.Application.Interface.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterUserAsync(RegisterUserDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<ApiResponse<bool>> LogoutAsync(Guid userId);
        Task<ApiResponse<CreatedOrganizationDto>> RegisterOrganizationAsync(CreateOrganizationDto dto, IFormFile logo);
    }
}
