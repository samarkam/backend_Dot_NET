namespace backend.DTO
{
    public class ArticleRequestDto
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }

        public required string Reference { get; set; }

        public int CategoryId { get; set; }
    }
}
