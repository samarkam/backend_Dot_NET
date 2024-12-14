namespace backend.DTO.order
{
    public class ArticleOrderDetailResponseDto
    {
        public int ArticleId { get; set; }

        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }
}
