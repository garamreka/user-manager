using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using UserManager.Api.DTOs;
using UserManager.Api.Services;

namespace UserManager.Api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;       
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>The users</returns>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id">String representation of the ObjectId</param>
        /// <returns>The user</returns>
        [HttpGet]
        [Route("{id:length(24)}")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User was not found, no changes were made in the database")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <returns>The string representation of the user's ObjectId</returns>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create(NewUserDto user)
        {
            var id = await _userService.AddUser(user);
            return Ok(id);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">The user to be updated</param>
        /// <returns>The status code</returns>
        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User was not found, no changes were made in the database")]
        [Authorize]
        public async Task<IActionResult> Update(UserDto user)
        {
            await _userService.UpdateUser(user.Id, user);
            return Ok();
        }

        /// <summary>
        /// Deletes the user
        /// </summary>
        /// <param name="id">The string representation of the user's ObjectId</param>
        /// <returns>The status code</returns>
        [HttpDelete]
        [Route("{id:length(24)}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "User was successfully deleted and resource is no longer available")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User was not found, no changes were made in the database")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUserById(id);
            return NoContent();
        }
    }
}
