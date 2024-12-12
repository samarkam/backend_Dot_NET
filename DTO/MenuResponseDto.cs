namespace backend.DTO
{
    public class MenuResponseDto
    {
        public int MenuId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<CategoryResponseDto> Categories { get; set; } = new List<CategoryResponseDto>();
    }
}