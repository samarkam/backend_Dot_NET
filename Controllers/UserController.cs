using backend.models;
using Microsoft.AspNetCore.Mvc;
using static backend.models.repository.IUserRepository;
    
namespace backend.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly IUserRepository _userRepository;

            // Injection du UserRepository via le constructeur
            public UserController(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            // Récupérer tous les utilisateurs
            [HttpGet]
            public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            {
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);  // Retourne la liste des utilisateurs
            }

            // Récupérer un utilisateur par son ID
            [HttpGet("{id}")]
            public async Task<ActionResult<User>> GetUser(int id)
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();  // Retourne une erreur 404 si l'utilisateur n'est pas trouvé
                return Ok(user);  // Retourne l'utilisateur trouvé
            }

            // Ajouter un nouvel utilisateur
            [HttpPost]
            public async Task<ActionResult<User>> CreateUser(User user)
            {
                if (user == null)
                {
                    return BadRequest("L'utilisateur ne peut pas être nul.");  // Validation de l'entrée
                }

                await _userRepository.AddUserAsync(user);  // Appel au repository pour créer l'utilisateur
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);  // Retourne un code 201 avec l'utilisateur créé
            }

            // Mettre à jour un utilisateur existant
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateUser(int id, User user)
            {
                if (id != user.UserId)
                    return BadRequest("L'ID de l'utilisateur ne correspond pas.");

                await _userRepository.UpdateUserAsync(user);  // Appel au repository pour mettre à jour l'utilisateur
                return NoContent();  // Retourne un code 204 en cas de succès sans contenu à renvoyer
            }

            
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();  // Retourne une erreur 404 si l'utilisateur n'est pas trouvé

                await _userRepository.DeleteUserAsync(id);  // Appel au repository pour supprimer l'utilisateur
                return NoContent();  // Retourne un code 204 en cas de succès
            }
        }
    }


