using MongoDB.Bson;
using UserManager.Api.Models;

namespace UserManager.Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUserById(ObjectId id);

        Task<ObjectId> AddUser(User user);

        Task<bool> UpdateUser(ObjectId id, User user);

        Task<bool> DeleteUserById(ObjectId id);
    }
}
