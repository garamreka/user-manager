using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManager.Api.Controllers;
using UserManager.Api.DTOs;
using UserManager.Api.Services;

namespace UserManager.Api.UnitTest
{
    public class UserControllerTests
    {
        private readonly Fixture _fixture;

        public UserControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            // Arrange
            var users = _fixture.CreateMany<UserDto>(5);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOkResult()
        {
            // Arrange
            var userId = "657c50438c61e8660f780ed2";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(new UserDto { Id = userId });

            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = await controller.GetById(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new UserDto { Id = userId });
        }

        [Fact]
        public async Task Create_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var user = _fixture.Create<NewUserDto>();

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.AddUser(user)).ReturnsAsync("newUserId");

            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = await controller.Create(user);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("newUserId");
        }

        [Fact]
        public async Task Update_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var user = new UserDto { Id = "validUserId" };
            var userServiceMock = new Mock<IUserService>();

            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = await controller.Update(user);

            // Assert
            result.Should().BeOfType<OkResult>();
            userServiceMock.Verify(x => x.UpdateUser(user.Id, user), Times.Once);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsNoContentResult()
        {
            // Arrange
            var userId = "validUserId";
            var userServiceMock = new Mock<IUserService>();

            var controller = new UserController(userServiceMock.Object);

            // Act
            var result = await controller.Delete(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            userServiceMock.Verify(x => x.DeleteUserById(userId), Times.Once);
        }
    }
}
