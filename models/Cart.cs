using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class Cart
    {
        public int CartId { get; set; }
        

        public required ICollection<CartItem> CartItems { get; set; }



        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public required User User { get; set; }
    }
}
