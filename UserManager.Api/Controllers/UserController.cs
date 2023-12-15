using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
            return Ok(users);
        }

        [HttpGet]
        [Route("{id:length(24)}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userRepository.GetUserById(ObjectId.Parse(id));

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var id = await _userRepository.AddUser(user);
            return Ok(id);
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            var success = await _userRepository.UpdateUser(user.Id, user);
            return success ? Ok() : NotFound();
        }

        [HttpDelete]
        [Route("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userRepository.DeleteUserById(ObjectId.Parse(id));
            return success ? NoContent() : NotFound();
        }
    }
}
