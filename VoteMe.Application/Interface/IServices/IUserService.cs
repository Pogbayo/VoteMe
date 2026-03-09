
using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.User;

namespace VoteMe.Application.Interface.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserAsync(Guid userId);
        Task<ApiResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid userId);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    }
}
