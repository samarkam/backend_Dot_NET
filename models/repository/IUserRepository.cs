using backend.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.models.repository
{
    public interface IUserRepository

    {


        public interface IUserRepository
        {
            Task<IEnumerable<User>> GetAllUsersAsync();
            Task<User> GetUserByIdAsync(int id);
            Task<User> CreateUserAsync(User user);
            Task UpdateUserAsync(User user);
            Task DeleteUserAsync(int id);
            Task AddUserAsync(User user);
        }
    }



}

