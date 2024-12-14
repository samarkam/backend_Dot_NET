using backend.DTO.user;
using System.Collections.ObjectModel;

namespace backend.DTO.order
{
    public class OrderWithUserResponseDto
    {

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }

        public UserDetailsDto UserDetails { get; set; }
        public required Collection<OrderDetailResponseDto> OrderDetails { get; set; }

    }
}
