using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManager.Api.Configurations;
using UserManager.Api.Models;
using UserManager.Api.Repositories;
using Xunit.Abstractions;

namespace UserManager.Api.UnitTest
{
    public class UserRepositoryTests : IClassFixture<MongoDbFixture>
    {
        private readonly Fixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly IMongoCollection<User> _userCollection;
        private readonly UserRepository _userRepository;
        private readonly IOptions<UserManagerDatabaseConfiguration> _configuration;

        public UserRepositoryTests(MongoDbFixture fixture, ITestOutputHelper output)
        {
            _fixture = new Fixture();
            _output = output;
            _userCollection = fixture.Database.GetCollection<User>("users");
            _configuration = Options.Create(fixture.DatabaseConfig);
            _userRepository = new UserRepository(fixture.Client, _configuration);
        }

        [Fact]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(x => x.Id)
                .Create();

            // Act
            var addedUserId = await _userRepository.AddUser(user);

            // Assert
            var retrievedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            retrievedUser.Should().NotBeNull();
            retrievedUser.Should().BeEquivalentTo(user);

            var result = await _userCollection.DeleteOneAsync(x => x.Id == addedUserId);
            result.DeletedCount.Should().Be(1);
        }

        [Fact]
        public async Task DeleteUserById_ShouldDeleteUserFromDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(x => x.Id)
                .Create();
            var addedUserId = await _userRepository.AddUser(user);

            var addedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            addedUser.Should().NotBeNull();

            // Act
            var result = await _userRepository.DeleteUserById(addedUserId);

            // Assert
            result.Should().BeTrue();
            var deletedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var users = _fixture.Build<User>()
                .Without(x => x.Id)
                .CreateMany(2);

            foreach (var user in users)
            {
                var addedUserId = await _userRepository.AddUser(user);
                var addedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
                addedUser.Should().NotBeNull();
            }

            // Act
            var retrievedUsers = await _userRepository.GetAllUsers();

            // Assert
            retrievedUsers.Should().NotBeNull();
            retrievedUsers.Should().HaveSameCount(users);
            retrievedUsers.Should().BeEquivalentTo(users);

            foreach (var user in retrievedUsers)
            {
                var result = await _userCollection.DeleteOneAsync(x => x.Id == user.Id);
                result.DeletedCount.Should().Be(1);
            }
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUserWithGivenId()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(x => x.Id)
                .Create();
            var addedUserId = await _userRepository.AddUser(user);

            var addedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            addedUser.Should().NotBeNull();

            // Act
            var retrievedUser = await _userRepository.GetUserById(addedUserId);

            // Assert
            retrievedUser.Should().NotBeNull();
            retrievedUser.Should().BeEquivalentTo(user);

            var result = await _userCollection.DeleteOneAsync(x => x.Id == addedUserId);
            result.DeletedCount.Should().Be(1);
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(x => x.Id)
                .With(x => x.UserName, "John Doe")
                .Create();
            var addedUserId = await _userRepository.AddUser(user);

            var addedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            addedUser.Should().NotBeNull();

            var updatedUser = _fixture.Build<User>()
                .Without(x => x.Id)
                .With(x => x.UserName, "Gipsz Jakab")
                .Create();
            updatedUser.Id = addedUserId;

            // Act
            var result = await _userRepository.UpdateUser(addedUserId, updatedUser);

            // Assert
            result.Should().BeTrue();
            var retrievedUser = await _userCollection.Find(x => x.Id == addedUserId).FirstOrDefaultAsync();
            retrievedUser.Should().NotBeNull();
            retrievedUser.Should().BeEquivalentTo(updatedUser);

            var deletedResult = await _userCollection.DeleteOneAsync(x => x.Id == addedUserId);
            deletedResult.DeletedCount.Should().Be(1);
        }
    }
}
