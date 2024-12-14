using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }

        public required string Reference { get; set; }

        public required bool  IsVisible { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public required Category Category { get; set; }
    }
}
