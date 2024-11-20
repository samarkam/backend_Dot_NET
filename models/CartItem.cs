using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class CartItem
    {


        public int CartItemId { get; set; }

        public required int Quantity { get; set; }



        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public virtual required Cart Cart { get; set; }



        public int ArticleId { get; set; }
        [ForeignKey(nameof(ArticleId))]

        public required Article Article { get; set; }

      


    }
}
