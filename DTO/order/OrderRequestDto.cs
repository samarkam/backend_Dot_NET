using System.Collections.ObjectModel;

namespace backend.DTO.order
{
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }

        public required Collection<OrderdetailRequestDto> OrderDetails { get; set; }
    }
}
