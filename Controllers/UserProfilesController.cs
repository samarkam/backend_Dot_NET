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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        /* [HttpGet]
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
                         UserId = up.UserId,

                         Email = up.User.Email,
                         UserName = up.User.UserName
                     }
                 })
                 .ToListAsync();

             return Ok(profiles);
         }*/

        /*  [HttpGet("{id}")]
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
                       UserId = profile.UserId,

                       Email = profile.User.Email,
                       UserName = profile.User.UserName
                   }
              });
          }
  */
        /*[HttpPut("{id}")]
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
                    UserId = profile.UserId,
                    Email = profile.User.Email,
                    UserName = profile.User.UserName
                }
            });
        }*/

        /*   [HttpPost]
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

               return  Ok(new UserProfileResponseDto
               {
                   UserProfileId = profile.UserProfileId,
                   Name = profile.Name,
                   PhoneNumber = profile.PhoneNumber,

                   Address = profile.Address,
                   UserId = profile.UserId,
                   User = new UserResponseDto
                   {
                       UserId = profile.UserId,
                       Email = profile.User.Email,
                       UserName = profile.User.UserName
                   }
               }); 
           }*/
        /*
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
                }*/



        [HttpPut("{UserId}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserProfuleUpdate request, int UserId)
        {
            // Find the user by ID, including their profile
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            // Validate the input data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for unique email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.UserId != UserId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email is already in use."
                });
            }

            // Update user details
            user.UserName = request.Name;
            user.Email = request.Email;

            // Check if the profile exists; if not, create a new profile
            if (user.UserProfile != null)
            {
                user.UserProfile.Name = request.Name;

                user.UserProfile.PhoneNumber = request.PhoneNumber;
                user.UserProfile.Address = request.Address;
            }
            else
            {
                // Create a new profile if it doesn't exist
                user.UserProfile = new UserProfile
                {
                    UserId = UserId,
                    User = user,
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    Orders = new List<Order>()
                };
            }

            await _context.SaveChangesAsync();

            return Ok("updated successfully");
        }
    }

}
