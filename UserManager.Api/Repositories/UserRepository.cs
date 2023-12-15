using MongoDB.Driver;
using UserManager.Api.Models;

namespace UserManager.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient client)
        {
            var database = client.GetDatabase("UserManagerDb");
            _users = database.GetCollection<User>("User");
        }

        public Task<bool> AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(string id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
