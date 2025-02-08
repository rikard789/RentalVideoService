using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using VideoRentalService.Dtos;
using VideoRentalService.Models;
using VideoRentalService.Services;

namespace VideoRentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        // Store signing keys per user session (userId -> key)
        public static readonly Dictionary<string, SymmetricSecurityKey> UserKeys = new Dictionary<string, SymmetricSecurityKey>();

        public AuthorizationController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // Login Endpoint
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto.Username == null || loginDto.Password == null)
            {
                return BadRequest("Wrong username or password.");
            }
            var user = await _userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password);
            if (user == null) return Unauthorized("Invalid username or password.");

            // Generate a unique key for this user session
            var signingKey = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32)); // 256-bit key
            var userId = Guid.NewGuid().ToString(); // Generate unique user session ID

            // Store the key for this session
            UserKeys[userId] = signingKey;

            // Generate JWT Token
            var token = GenerateJwtToken(userId, user, signingKey);

            return Ok(new { token, user.Role, user.UserId });
        }

        // Logout Endpoint
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            // Remove the user's signing key to invalidate their token
            if (UserKeys.ContainsKey(userId))
            {
                UserKeys.Remove(userId);
                return Ok("User logged out successfully.");
            }

            return NotFound("User session not found.");
        }

        private string GenerateJwtToken(String userId, User user, SymmetricSecurityKey signingKey)
        {
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", userId)
            };

            var token = new JwtSecurityToken(
                issuer: "VideoRentalService",
                audience: "VideoRentalUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
