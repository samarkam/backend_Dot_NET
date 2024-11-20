using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }

        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]

        public required User User { get; set; }

        public required ICollection<Order> Orders { get; set; }

    }
}
