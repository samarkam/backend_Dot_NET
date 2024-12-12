using backend.models;
using backend.models.repository.backend.Repository;
using backend.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
            private readonly IUserProfileRepository _userProfileRepository;

            // Injection du repository dans le contrôleur
            public UserProfileController(IUserProfileRepository userProfileRepository)
            {
                _userProfileRepository = userProfileRepository;
            }

            // GET: api/userprofile/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<UserProfile>> GetUserProfileById(int id)
            {
                var userProfile = await _userProfileRepository.GetUserProfileByIdAsync(id);

                if (userProfile == null)
                {
                    return NotFound();  // Si le profil n'est pas trouvé, retourner 404
                }

                return Ok(userProfile);  // Retourner le profil trouvé
            }

            // POST: api/userprofile
            [HttpPost]
            public async Task<ActionResult<UserProfile>> CreateUserProfile(UserProfile userProfile)
            {
                if (userProfile == null)
                {
                    return BadRequest("Le profil utilisateur ne peut pas être nul.");  // Validation de l'entrée
                }

                // Ajouter le profil dans la base de données
                await _userProfileRepository.AddUserProfileAsync(userProfile);

                // Retourner le profil créé avec un code HTTP 201 (Created)
                return CreatedAtAction(nameof(GetUserProfileById), new { id = userProfile.UserProfileId }, userProfile);
            }

            // PUT: api/userprofile/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateUserProfile(int id, UserProfile userProfile)
            {
                if (id != userProfile.UserProfileId)
                {
                    return BadRequest("Les IDs ne correspondent pas.");  // Validation de l'ID
                }

                var existingUserProfile = await _userProfileRepository.GetUserProfileByIdAsync(id);
                if (existingUserProfile == null)
                {
                    return NotFound();  // Si le profil n'existe pas, retourner 404
                }

                // Mettre à jour le profil utilisateur
                await _userProfileRepository.UpdateUserProfileAsync(userProfile);

                return NoContent();  // Retourner 204 No Content pour indiquer que la mise à jour a réussi
            }

            // DELETE: api/userprofile/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUserProfile(int id)
            {
                var userProfile = await _userProfileRepository.GetUserProfileByIdAsync(id);
                if (userProfile == null)
                {
                    return NotFound();  // Si le profil n'existe pas, retourner 404
                }

                // Supprimer le profil utilisateur
                await _userProfileRepository.DeleteUserProfileAsync(id);

                return NoContent();  // Retourner 204 No Content pour indiquer que la suppression a réussi
            }

            // GET: api/userprofile
            [HttpGet]
            public async Task<ActionResult<IEnumerable<UserProfile>>> GetAllUserProfiles()
            {
                var userProfiles = await _userProfileRepository.GetAllUserProfilesAsync();
                return Ok(userProfiles);  // Retourner la liste des profils d'utilisateur
            }
        }
    }



