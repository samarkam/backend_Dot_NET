using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public required string Name { get; set; }

        public int MenuId { get; set; }

        [ForeignKey(nameof(MenuId))]
        public required Menu Menu { get; set; }


        public required ICollection<Article> Articles { get; set; }

    }
}
