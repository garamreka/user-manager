using Mongo2Go;
using MongoDB.Driver;
using UserManager.Api.Configurations;

namespace UserManager.Api.UnitTest
{
    public class MongoDbFixture : IDisposable
    {
        private readonly MongoDbRunner _runner;

        public MongoDbFixture()
        {
            _runner = MongoDbRunner.Start();
            Client = new MongoClient(_runner.ConnectionString);
            Database = Client.GetDatabase("testdb");

            DatabaseConfig = new UserManagerDatabaseConfiguration
            {
                DatabaseName = "testdb",
                CollectionName = "users"
            };
        }

        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }
        public UserManagerDatabaseConfiguration DatabaseConfig { get; }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}
