using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoRentalService.Models;
using VideoRentalService.Services;

namespace VideoRentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // List all users
        [HttpGet("allUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Get user by ID
        [HttpGet("getUser/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        // Get user by Username
        [HttpGet("getUserUsername/{username}")]
        public async Task<ActionResult<List<User>>> GetUserByUsername(string username)
        {
            var user = await _userService.FindUsersByUsernameAsync(username);
            if (user == null) return NotFound($"User with username {username} not found.");

            return Ok(user);
        }

        // Create a new user
        [HttpPost("createUser")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var newUser = await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, newUser);
        }

        // Update an existing user by id
        [HttpPut("updateUser/{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, User updatedUser)
        {
            var user = await _userService.UpdateUserAsync(id, updatedUser);
            if (user == null) return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        // Delete a user by id
        [HttpDelete("deleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound($"User with ID {id} not found.");

            return NoContent();
        }
    }
}
