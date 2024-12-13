namespace backend.DTO
{
    public class OrderDetailResponseDto
    {
        public int OrderDetailId { get; set; }
        public int ArticleId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
