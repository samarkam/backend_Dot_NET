using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class Order
    {
        public int OrderId { get; set; }
    
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public OrderStatus Status { get; set; }


        public required
            ICollection<OrderDetail> OrderDetails { get; set; }


        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public required User User{ get; set; }



    }


    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}
