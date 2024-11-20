using System.ComponentModel.DataAnnotations.Schema;

namespace backend.models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }


        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]

        public required Category Category { get; set; }
    }
}
