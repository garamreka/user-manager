using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserManager.Api.DTOs;
using UserManager.Api.IntegrationTest;
using UserManager.Api.Middlewares;
using UserManager.Api.Models;
using UserManager.Api.Repositories;
using UserManager.Api.Services;

namespace UserManager.Api.IntegrationTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Fixture _fixture;
        private readonly WebApplicationFactory<Program> _factory;
        protected Mock<IUserRepository> _mockUserRepository = new();

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _fixture = new Fixture();
            _factory = SetupFactory(factory);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsUsers()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var users = _fixture.Build<User>().With(x => x.Id, ObjectId.GenerateNewId()).CreateMany(2);
            _mockUserRepository.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);

            // Act
            var response = await client.GetAsync("api/v1/users");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var content = await response.Content.ReadAsStringAsync();
            var userDtos = JsonConvert.DeserializeObject<List<UserDto>>(content);
            userDtos.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUserById_WithValidId_ReturnsUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            var user = _fixture.Build<User>().With(x => x.Id, userId).Create();
            _mockUserRepository.Setup(repo => repo.GetUserById(It.IsAny<ObjectId>())).ReturnsAsync(user);

            // Act
            var id = userId.ToString();
            var response = await client.GetAsync($"api/v1/users/{id}");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var content = await response.Content.ReadAsStringAsync();
            
            var userDto = JsonConvert.DeserializeObject<UserDto>(content);
            userDto.Id.Should().Be(userId.ToString());
            userDto.Name.Should().Be(user.Name);
            userDto.UserName.Should().Be(user.UserName);
            userDto.Email.Should().Be(user.Email);
            userDto.Phone.Should().Be(user.Phone);
            userDto.Website.Should().Be(user.Website);
            userDto.Address.Street.Should().Be(user.Address.Street);
            userDto.Address.Suite.Should().Be(user.Address.Suite);
            userDto.Address.City.Should().Be(user.Address.City);
            userDto.Address.ZipCode.Should().Be(user.Address.ZipCode);
            userDto.Address.Geo.Lat.Should().Be(user.Address.Geo.Lat);
            userDto.Address.Geo.Lng.Should().Be(user.Address.Geo.Lng);
            userDto.Company.Name.Should().Be(user.Company.Name);
            userDto.Company.CatchPhrase.Should().Be(user.Company.CatchPhrase);
            userDto.Company.Bs.Should().Be(user.Company.Bs);
        }

        [Fact]
        public async Task GetUserById_WithNoResource_ReturnsUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            User user = null;
            _mockUserRepository.Setup(repo => repo.GetUserById(It.IsAny<ObjectId>())).ReturnsAsync(user);

            // Act
            var id = userId.ToString();
            var response = await client.GetAsync($"api/v1/users/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUserById_WithInvalidId_ReturnsUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            // Act
            var response = await client.GetAsync($"api/v1/users/1234567890óüúő1234567890");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<Error>(content);
            error.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUserById_WithValidId_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            _mockUserRepository.Setup(repo => repo.DeleteUserById(It.IsAny<ObjectId>())).ReturnsAsync(true);

            // Act
            var id = userId.ToString();
            var response = await client.DeleteAsync($"api/v1/users/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task CreateUser_ReturnsUserId()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            var user = new NewUserDto
            {
                Name = "Clementina DuBuque",
                UserName = "Moriah.Stanton",
                Email = "Rey.Padberg@karina.biz",
                Phone = "024-648-3804",
                Website = "ambrose.net",
                Address = new AddressDto
                {
                    Street = "Kattie Turnpike",
                    Suite = "Suite 198",
                    City = "Lebsackbury",
                    ZipCode = "31428-2261",
                    Geo = new GeoDto
                    {
                        Lat = "-38.2386",
                        Lng = "57.2232"
                    }
                },
                Company = new CompanyDto
                {
                    Name = "Hoeger LLC",
                    CatchPhrase = "Centralized empowering task-force",
                    Bs = "target end-to-end models"
                }
            };
            _mockUserRepository.Setup(repo => repo.AddUser(It.IsAny<User>())).ReturnsAsync(userId);

            // Act
            var response = await client.PostAsJsonAsync($"api/v1/users", user);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Be(userId.ToString());
        }

        [Fact]
        public async Task UpdateUser_WithFullUser_Returns()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            var user = new UserDto
            {
                Id = userId.ToString(),
                Name = "Clementina DuBuque",
                UserName = "Moriah.Stanton",
                Email = "Rey.Padberg@karina.biz",
                Phone = "024-648-3804",
                Website = "ambrose.net",
                Address = new AddressDto
                {
                    Street = "Kattie Turnpike",
                    Suite = "Suite 198",
                    City = "Lebsackbury",
                    ZipCode = "31428-2261",
                    Geo = new GeoDto
                    {
                        Lat = "-38.2386",
                        Lng = "57.2232"
                    }
                },
                Company = new CompanyDto
                {
                    Name = "Hoeger LLC",
                    CatchPhrase = "Centralized empowering task-force",
                    Bs = "target end-to-end models"
                }
            };
            _mockUserRepository.Setup(repo => repo.UpdateUser(It.IsAny<ObjectId>(), It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var response = await client.PutAsJsonAsync($"api/v1/users", user);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUser_WithPartialUser_Returns()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

            var userId = ObjectId.GenerateNewId();
            var user = new UserDto
            {
                Id = userId.ToString(),
                Email = "Rey.Padberg@karina.biz",
                Phone = "024-648-3804",
                Website = "ambrose.net"
            };
            _mockUserRepository.Setup(repo => repo.UpdateUser(It.IsAny<ObjectId>(), It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var response = await client.PutAsJsonAsync($"api/v1/users", user);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        private WebApplicationFactory<Program> SetupFactory(WebApplicationFactory<Program> factory)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockUserRepository.Object);
                    services.AddAutoMapper(typeof(UserMapperProfile));
                    services.AddScoped<IUserService, UserService>();

                    services.AddAuthentication(o =>
                    {
                        o.DefaultAuthenticateScheme = "Test";
                        o.DefaultChallengeScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });
                });
            });
        }
    }
}
