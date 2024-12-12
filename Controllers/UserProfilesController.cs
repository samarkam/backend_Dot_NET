using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.models;
using backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.DTO.user;
using backend.models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileResponseDto>>> GetUserProfiles()
        {
            var profiles = await _context.UserProfiles
                .Include(up => up.User)
                .Select(up => new UserProfileResponseDto
                {
                    UserProfileId = up.UserProfileId,
                    Name = up.Name,
                    PhoneNumber = up.PhoneNumber,
                    Address = up.Address,
                    UserId = up.UserId,
                    User =new UserResponseDto {
                         Email = up.User.Email,
                        UserName = up.User.UserName
                    }
                })
                .ToListAsync();

            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileResponseDto>> GetUserProfile(int id)
        {
            var profile = await _context.UserProfiles
                .Include(up => up.User)
                .FirstOrDefaultAsync(up => up.UserProfileId == id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(new UserProfileResponseDto
            {
                UserProfileId = profile.UserProfileId,
                Name = profile.Name,
                PhoneNumber = profile.PhoneNumber,
               
                Address = profile.Address,
                UserId = profile.UserId,
                 User = new UserResponseDto
                 {
                     Email = profile.User.Email,
                     UserName = profile.User.UserName
                 }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProfile(int id, UserProfileRequestDto userProfileRequestDto)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(userProfileRequestDto.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {userProfileRequestDto.UserId} not found.");
            }
            profile.Name = userProfileRequestDto.Name;
            profile.PhoneNumber = userProfileRequestDto.PhoneNumber;
            profile.Address = userProfileRequestDto.Address;
            profile.UserId = userProfileRequestDto.UserId;
            profile.User = user;
            _context.Entry(profile).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new UserProfileResponseDto
            {
                UserProfileId = profile.UserProfileId,
                Name = profile.Name,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                UserId = profile.UserId,
                User = new UserResponseDto
                {
                    Email = profile.User.Email,
                    UserName = profile.User.UserName
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserProfileResponseDto>> PostUserProfile(UserProfileRequestDto userProfileRequestDto)
        {
            var user = await _context.Users.FindAsync(userProfileRequestDto.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {userProfileRequestDto.UserId} not found.");
            }

            var profile = new UserProfile
            {
                Name = userProfileRequestDto.Name,
                PhoneNumber = userProfileRequestDto.PhoneNumber,
                Address = userProfileRequestDto.Address,
                UserId = userProfileRequestDto.UserId,
                User = user,
                Orders = new List<Order>()
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserProfile), new { id = profile.UserProfileId }, profile);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(int id)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User profile deleted successfully." });
        }
    }
}
