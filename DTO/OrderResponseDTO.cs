using backend.models;
using System.Collections.ObjectModel;

namespace backend.DTO
{
    public class OrderResponseDTO
    {

        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public required Collection<OrderDetailResponseDto> OrderDetails { get; set; }


    }
}