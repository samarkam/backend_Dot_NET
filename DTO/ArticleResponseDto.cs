namespace backend.DTO
{
    public class ArticleResponseDto
    {

        public int ArticleId { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public int CategoryId { get; set; }

        public string? Reference { get; set; }
        public CategoryResponseDto? Category { get; set; }
    }
}
