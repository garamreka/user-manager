using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using Moq;
using UserManager.Api.CustomExceptions;
using UserManager.Api.DTOs;
using UserManager.Api.Models;
using UserManager.Api.Repositories;
using UserManager.Api.Services;

namespace UserManager.Api.UnitTest
{
    public class UserServiceTests
    {
        private readonly Fixture _fixture;

        public UserServiceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task AddUser_ValidUserDto_ReturnsUserId()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();

            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

            var userDto = _fixture.Create<NewUserDto>();
            var userId = ObjectId.GenerateNewId();

            mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(new User() { Id = userId });
            userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<User>())).ReturnsAsync(userId);

            // Act
            var result = await userService.AddUser(userDto);

            // Assert
            result.Should().Be(userId.ToString());
        }

        [Fact]
        public async Task DeleteUserById_ValidId_SuccessfulDeletion()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();

            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var userId = ObjectId.GenerateNewId();

            userRepositoryMock.Setup(repo => repo.DeleteUserById(userId)).ReturnsAsync(true);

            // Act
            Func<Task> action = async () => await userService.DeleteUserById(userId.ToString());

            // Assert
            await action.Should().NotThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteUserById_NoResource_ThrowsNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var invalidUserId = ObjectId.GenerateNewId();

            userRepositoryMock.Setup(repo => repo.DeleteUserById(invalidUserId)).ReturnsAsync(false);

            // Act
            Func<Task> action = async () => await userService.DeleteUserById(invalidUserId.ToString());

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteUserById_InvalidId_ThrowsInvalidOperationException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

            // Act
            Func<Task> action = async () => await userService.DeleteUserById("invalidId");

            // Assert
            userRepositoryMock.Verify(repo => repo.DeleteUserById(It.IsAny<ObjectId>()), Times.Never);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task DeleteUserById_EmptyId_ThrowsInvalidOperationException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

            // Act
            Func<Task> action = async () => await userService.DeleteUserById(string.Empty);

            // Assert
            userRepositoryMock.Verify(repo => repo.DeleteUserById(It.IsAny<ObjectId>()), Times.Never);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsUserDtos()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();

            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var users = _fixture.Build<User>().With(x => x.Id, ObjectId.GenerateNewId()).CreateMany(5);
            var userDtos = _fixture.CreateMany<UserDto>(5);

            userRepositoryMock.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);
            mapperMock.Setup(mapper => mapper.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await userService.GetAllUsers();

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUserDto()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var userId = ObjectId.GenerateNewId();
            var user = _fixture.Build<User>().With(x => x.Id, userId).Create();
            var userDto = _fixture.Create<UserDto>();

            userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
            mapperMock.Setup(mapper => mapper.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await userService.GetUserById(userId.ToString());

            // Assert
            result.Should().Be(userDto);
        }

        [Fact]
        public async Task GetUserById_NoResource_ThrowsNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var invalidUserId = ObjectId.GenerateNewId();

            userRepositoryMock.Setup(repo => repo.GetUserById(invalidUserId)).ReturnsAsync((User)null);

            // Act
            Func<Task> action = async () => await userService.GetUserById(invalidUserId.ToString());

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetUserById_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

            // Act
            Func<Task> action = async () => await userService.GetUserById("invalidId");

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
            userRepositoryMock.Verify(repo => repo.GetUserById(It.IsAny<ObjectId>()), Times.Never);
        }

        [Fact]
        public async Task GetUserById_EmptyId_ThrowsNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

            // Act
            Func<Task> action = async () => await userService.GetUserById(string.Empty);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
            userRepositoryMock.Verify(repo => repo.GetUserById(It.IsAny<ObjectId>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_ValidIdAndUserDto_SuccessfulUpdate()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var userId = ObjectId.GenerateNewId();
            var userDto = new UserDto();

            mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(new User());
            userRepositoryMock.Setup(repo => repo.UpdateUser(userId, It.IsAny<User>())).ReturnsAsync(true);

            // Act
            Func<Task> action = async () => await userService.UpdateUser(userId.ToString(), userDto);

            // Assert
            await action.Should().NotThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateUser_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
            var invalidUserId = ObjectId.GenerateNewId();
            var userDto = _fixture.Create<UserDto>();

            userRepositoryMock.Setup(repo => repo.UpdateUser(invalidUserId, It.IsAny<User>())).ReturnsAsync(false);
            mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(new User());

            // Act
            Func<Task> action = async () => await userService.UpdateUser(invalidUserId.ToString(), userDto);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}
