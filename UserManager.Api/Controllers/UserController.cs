using Microsoft.AspNetCore.Mvc;
using UserManager.Api.Models;
using UserManager.Api.Repositories;

namespace UserManager.Api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;       
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllUsers();
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            throw new NotImplementedException();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var sucess = await _userRepository.AddUser(user);
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            var sucess = await _userRepository.UpdateUser(user.Id, user);
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var sucess = await _userRepository.DeleteUserById(id);
            throw new NotImplementedException();
        }
    }
}
