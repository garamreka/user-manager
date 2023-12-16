using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManager.Api.Configurations;
using UserManager.Api.Models;

namespace UserManager.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(
            IMongoClient client,
            IOptions<UserManagerDatabaseConfiguration> configuration)
        {
            var database = client.GetDatabase(configuration.Value.DatabaseName);
            this._users = database.GetCollection<User>(configuration.Value.CollectionName);
        }

        public async Task<ObjectId> AddUser(User user)
        {
            await _users.InsertOneAsync(user);

            return user.Id;
        }

        public async Task<bool> DeleteUserById(ObjectId id)
        {
            var result = await _users.DeleteOneAsync(x => x.Id == id);

            return result.DeletedCount == 1;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetUserById(ObjectId id)
        {
            return await _users.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUser(ObjectId id, User user)
        {
            var result = await _users.ReplaceOneAsync(x => x.Id == id, user);

            return result.ModifiedCount == 1;
        }
    }
}
