
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.User;

namespace VoteMe.Application.Interface.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserAsync(Guid userId);
        //Task<ApiResponse<OrganizationUserDto>> UpdateUserAsync(UpdateUserDto dto);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid userId, Guid organizationId);
        Task<ApiResponse<IEnumerable<OrganizationUserDto>>> GetAllUsersAsync(Guid organizationId , int page = 1, int pageSize = 20);
    }
}
