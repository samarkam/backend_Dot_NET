namespace backend.models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public required ICollection<Category> Categorys { get; set; }

    }
}
