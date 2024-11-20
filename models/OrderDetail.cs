using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
    
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }    
        
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]

        public required Order Order{ get; set; }        
        
        
        public int ArticleId { get; set; }

        [ForeignKey(nameof(ArticleId))]

        public required Article Article { get; set; }
    }
}
