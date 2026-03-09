using VoteMe.Application.Common;
using VoteMe.Application.DTOs.User;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class UserService : IUserService
    {
        public Task<ApiResponse<bool>> DeleteUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync(int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserDto>> GetUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
