using System.Collections.ObjectModel;

namespace backend.DTO
{
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }

        public required Collection<OrderDetailRequestDto> OrderDetails { get; set; }
    }
}
