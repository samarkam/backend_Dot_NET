using backend.DTO;
using backend.models;
using Microsoft.EntityFrameworkCore;

namespace backend.REPOSITORY.IMPL
{
    public class ArticleRepository :IArticleRepository
    {


        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArticleResponseDto>> GetArticlesAsync()
        {
            return await _context.Articles
                .Include(a => a.Category)
                .Select(a => new ArticleResponseDto
                {
                    ArticleId = a.ArticleId,
                    Name = a.Name,
                    Price = a.Price,
                    Reference = a.Reference,
                    CategoryId = a.CategoryId,
                    IsVisible = a.IsVisible,

                    Category = new CategoryResponseDto
                    {
                        CategoryId = a.Category.CategoryId,
                        Name = a.Category.Name,
                        MenuId = a.Category.MenuId
                    }
                })
                .ToListAsync();
        }


        public async Task<ArticleResponseDto?> GetArticleByIdAsync(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
            {
                return null;
            }

            return new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Name = article.Name,
                Price = article.Price,
                Reference = article.Reference,
                CategoryId = article.CategoryId,
                IsVisible = article.IsVisible,

                Category = new CategoryResponseDto
                {
                    CategoryId = article.Category.CategoryId,
                    Name = article.Category.Name,
                    MenuId = article.Category.MenuId
                }
            };
        }


        public async Task<ArticleResponseDto?> UpdateArticleAsync(int id, ArticleRequestDto articleRequestDto)
        {
            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                return null; // Article not found
            }

            // Check if the Category exists
            var existingCategory = await _context.Categories.FindAsync(articleRequestDto.CategoryId);
            if (existingCategory == null)
            {
                throw new ArgumentException("The specified CategoryId does not exist.");
            }

            // Update the article properties
            existingArticle.Name = articleRequestDto.Name;
            existingArticle.Price = articleRequestDto.Price;
            existingArticle.CategoryId = articleRequestDto.CategoryId;
            existingArticle.Reference = articleRequestDto.Reference;

            // Save changes to the database
            _context.Entry(existingArticle).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return the updated article as a DTO
            return new ArticleResponseDto
            {
                ArticleId = existingArticle.ArticleId,
                Name = existingArticle.Name,
                Price = existingArticle.Price,
                CategoryId = existingArticle.CategoryId,
                IsVisible = existingArticle.IsVisible,

                Category = new CategoryResponseDto
                {
                    CategoryId = existingCategory.CategoryId,
                    Name = existingCategory.Name,
                    MenuId = existingCategory.MenuId
                },
                Reference = existingArticle.Reference
            };
        }

        public async Task<ArticleResponseDto> CreateArticleAsync(ArticleRequestDto articleRequestDto)
        {
            // Check if the referenced Category exists
            var category = await _context.Categories.FindAsync(articleRequestDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category with ID {articleRequestDto.CategoryId} not found.");
            }

            // Map the DTO to the Article entity
            var article = new Article
            {
                Name = articleRequestDto.Name,
                Price = articleRequestDto.Price,
                CategoryId = articleRequestDto.CategoryId,
                Category = category, 
                IsVisible= true,
                Reference = articleRequestDto.Reference
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            // Return the created article as a DTO
            return new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Name = article.Name,
                Price = article.Price,
                CategoryId = article.CategoryId,
                IsVisible= article.IsVisible,
                Category = new CategoryResponseDto
                {
                    CategoryId = article.Category.CategoryId,
                    Name = article.Category.Name,
                    MenuId = article.Category.MenuId
                },
                Reference = article.Reference
            };
        }


        public async Task<bool> DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return false; // Article not found
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true; // Article deleted successfully
        }


        public async Task<(IEnumerable<ArticleResponseDto> Articles, int TotalCount)> GetArticlesPaginateAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page and pageSize must be greater than zero.");
            }

            var totalArticlesCount = await _context.Articles.CountAsync();

            var articles = await _context.Articles
                .Include(a => a.Category)
                .Select(a => new ArticleResponseDto
                {
                    ArticleId = a.ArticleId,
                    Name = a.Name,
                    Price = a.Price,
                    Reference = a.Reference,
                    CategoryId = a.CategoryId,
                    IsVisible = a.IsVisible,

                    Category = new CategoryResponseDto
                    {
                        CategoryId = a.Category.CategoryId,
                        Name = a.Category.Name,
                        MenuId = a.Category.MenuId
                    }
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (articles, totalArticlesCount);
        }



        public async Task UpdateArticleAsyncVisibility(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }
    }
}
