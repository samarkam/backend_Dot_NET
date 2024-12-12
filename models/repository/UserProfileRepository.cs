
using backend.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.models.repository
{
    
namespace backend.Repository
    {
        public class UserProfileRepository : IUserProfileRepository
        {
            private readonly ApplicationDbContext _context;

            public UserProfileRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            // Récupérer tous les profils d'utilisateurs
            public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync()
            {
                return await _context.UserProfiles
                                     .Include(up => up.User)    // Inclure la relation avec User
                                     .Include(up => up.Orders)  // Inclure la relation avec Orders si nécessaire
                                     .ToListAsync();
            }

            // Récupérer un profil d'utilisateur par son ID
            public async Task<UserProfile> GetUserProfileByIdAsync(int userProfileId)
            {
                return await _context.UserProfiles
                                     .Include(up => up.User)
                                     .Include(up => up.Orders)
                                     .FirstOrDefaultAsync(up => up.UserProfileId == userProfileId);
            }

            // Ajouter un nouveau profil utilisateur
            public async Task AddUserProfileAsync(UserProfile userProfile)
            {
                await _context.UserProfiles.AddAsync(userProfile);
                await _context.SaveChangesAsync();
            }

            // Mettre à jour un profil utilisateur existant
            public async Task UpdateUserProfileAsync(UserProfile userProfile)
            {
                _context.UserProfiles.Update(userProfile);
                await _context.SaveChangesAsync();
            }

            // Supprimer un profil utilisateur
            public async Task DeleteUserProfileAsync(int userProfileId)
            {
                var userProfile = await _context.UserProfiles.FindAsync(userProfileId);
                if (userProfile != null)
                {
                    _context.UserProfiles.Remove(userProfile);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

}

