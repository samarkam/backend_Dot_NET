using System;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO;
using backend.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend.DTO.user;
using Microsoft.CodeAnalysis.Scripting;
using backend.services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Configuration;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtAuthService _jwtAuth;
        private readonly IConfiguration _configuration;


        public AuthController(ApplicationDbContext context, JwtAuthService jwtAuth, IConfiguration configuration)
        {
            _context = context;
            _jwtAuth = jwtAuth;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == userRegistrationDto.Email);

            if (existingUser != null)
            {
                return Conflict("Email already exists.");
            }

            var user = new User
            {
                Email = userRegistrationDto.Email,
                UserName = userRegistrationDto.UserName,
                Avatar = userRegistrationDto.Avatar,
                Role = "CLIENT",
                Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                UserProfile = null

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtAuth.GenerateToken(user.UserName, user.UserId.ToString());

            return Ok(new { token });
        }

        //[HttpGet("user")]
        //[Authorize]
        //public ActionResult<UserResponseDto> GetUser()
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var user = _context.Users
        //        .Include(u => u.UserProfile)
        //        .FirstOrDefault(u => u.UserId == userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    return Ok(new UserResponseDto
        //    {
        //        UserId = user.UserId,
        //        UserName = user.UserName,
        //        UserProfile = new UserProfileResponseDto
        //        {
        //            Name = user.UserProfile.Name,
        //            PhoneNumber = user.UserProfile.PhoneNumber,
        //            Email = user.UserProfile.Email,
        //            Address = user.UserProfile.Address
        //        }
        //    });
        //}

        [HttpPost("validate-token")]
        public IActionResult ValidateToken()
        {
            try
            {
                // Extract the token from the Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new { message = "Token not provided" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Validate the token (replace with your validation logic)
                var tokenHandler = new JwtSecurityTokenHandler();
                string secretKey = _configuration["Jwt:Key"]
                    ?? throw new ArgumentNullException("Jwt:Key", "JWT secret key is not configured.");

                // Convert the secret key to a byte array
                var key = Encoding.UTF8.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "your-issuer", // Replace with your issuer
                    ValidAudience = "your-audience", // Replace with your audience
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                // Extract claims or user info from the validated token
                var jwtToken = validatedToken as JwtSecurityToken;
                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User ID claim is missing from the token.");
                }

                // Optionally, fetch user details from the database
                var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                return Ok(new
                {
                    message = "Token is valid",
                    user = new
                    {
                        user.UserId,
                        user.UserName,
                        user.Email
                    }
                });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { message = "Token is expired" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }





        [HttpPost("check-admin")]
        public async Task<ActionResult> CheckAdmin([FromBody] EmailDto emailDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailDto.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Role == "ADMIN")
            {
                return Ok(new { isAdmin = true });
            }
            else
            {
                return Ok(new { isAdmin = false });
            }
        }


    }
}
