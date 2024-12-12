namespace backend.DTO
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }

        public required string Name { get; set; }

        public int MenuId { get; set; }

        public ICollection<ArticleResponseDto> Articles { get; set; } = new List<ArticleResponseDto>();

    }
}
