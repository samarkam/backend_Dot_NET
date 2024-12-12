using backend.models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace backend.models.repository
{
    
namespace backend.Repository
    {
        public interface IUserProfileRepository
        {
            Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync();
            Task<UserProfile> GetUserProfileByIdAsync(int userProfileId);
            Task AddUserProfileAsync(UserProfile userProfile);
            Task UpdateUserProfileAsync(UserProfile userProfile);
            Task DeleteUserProfileAsync(int userProfileId);
        }
    }

}

