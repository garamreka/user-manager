using AutoMapper;
using MongoDB.Bson;
using UserManager.Api.CustomExceptions;
using UserManager.Api.DTOs;
using UserManager.Api.Models;
using UserManager.Api.Repositories;

namespace UserManager.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
        }

        public async Task<string> AddUser(NewUserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var userId = await _userRepository.AddUser(user);

            return userId.ToString();
        }

        public async Task DeleteUserById(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new InvalidOperationException();
            }

            var success = await _userRepository.DeleteUserById(objectId);

            if (!success)
            {
                throw new NotFoundException();
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserById(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new InvalidOperationException();
            }

            var user = await _userRepository.GetUserById(objectId);

            return user is null ? throw new NotFoundException() : _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUser(string id, UserDto user)
        {
            if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new InvalidOperationException();
            }

            var userToUpdate = _mapper.Map<User>(user);
            var success = await _userRepository.UpdateUser(objectId, userToUpdate);

            if (!success)
            {
                throw new NotFoundException();
            }
        }
    }
}
