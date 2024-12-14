using System.Collections.ObjectModel;

namespace backend.DTO.order
{
    public class OrderResponseDto
    {

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public required Collection<OrderDetailResponseDto> OrderDetails { get; set; }

    }
}
