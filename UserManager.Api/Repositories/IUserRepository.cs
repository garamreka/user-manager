using UserManager.Api.Models;

namespace UserManager.Api.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();

        Task<User> GetUserById(string id);

        Task<bool> AddUser(User user);

        Task<bool> UpdateUser(string id, User user);

        Task<bool> DeleteUserById(string id);
    }
}
