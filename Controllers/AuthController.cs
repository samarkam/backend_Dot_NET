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

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtAuthService _jwtAuth;

        public AuthController(ApplicationDbContext context, JwtAuthService jwtAuth)
        {
            _context = context;
            _jwtAuth = jwtAuth;
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

            var token = _jwtAuth.GenerateToken( user.UserName);

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

        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = _context.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                return Ok(new UserProfileResponseDto                 {
                   
                    Name = user.UserName,
                    PhoneNumber = user.UserProfile?.PhoneNumber,
                    Address = user.UserProfile?.Address,
                    User = new UserResponseDto
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email,

                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while validating the token." +ex);
            }
        }
    }
}
