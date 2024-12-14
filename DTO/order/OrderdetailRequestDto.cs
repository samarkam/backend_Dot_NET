namespace backend.DTO.order
{
    public class OrderdetailRequestDto
    {

        public int ArticleId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
