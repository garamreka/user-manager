using UserManager.Api.DTOs;

namespace UserManager.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();

        Task<UserDto> GetUserById(string id);

        Task<string> AddUser(NewUserDto user);

        Task UpdateUser(string id, UserDto user);

        Task DeleteUserById(string id);
    }
}
