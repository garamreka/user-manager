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
            var success = await _userRepository.DeleteUserById(ObjectId.Parse(id));

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
            var user = await _userRepository.GetUserById(ObjectId.Parse(id));

            if (user is null)
            {
                throw new NotFoundException();
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUser(string id, UserDto user)
        {
            var userToUpdate = _mapper.Map<User>(user);
            var success = await _userRepository.UpdateUser(ObjectId.Parse(id), userToUpdate);

            if (!success)
            {
                throw new NotFoundException();
            }
        }
    }
}
