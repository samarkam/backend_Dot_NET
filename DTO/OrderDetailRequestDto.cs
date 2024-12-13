namespace backend.DTO
{
    public class OrderDetailRequestDto
    {
        public int ArticleId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
