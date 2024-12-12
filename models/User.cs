using System.ComponentModel.DataAnnotations;

namespace backend.models
{
    public class User
    {
        public int UserId { get; set; }

        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Avatar { get; set; }
        public required string Role { get; set; }



        public UserProfile? UserProfile { get; set; }

    }
}
