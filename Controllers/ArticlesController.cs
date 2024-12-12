using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.models;
using backend.DTO;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleResponseDto>>> GetArticles()
        {
            var articles = await _context.Articles
                .Include(a => a.Category)
                .Select(a => new ArticleResponseDto
                {
                    ArticleId = a.ArticleId,
                    Name = a.Name,
                    Price = a.Price,
                    Reference = a.Reference,

                    CategoryId = a.CategoryId,
                    Category = new CategoryResponseDto
                    {
                        CategoryId = a.Category.CategoryId,
                        Name = a.Category.Name,
                        MenuId = a.Category.MenuId
                    }
                })
                .ToListAsync();

            return Ok(articles);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleResponseDto>> GetArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            return new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Name = article.Name,
                Price = article.Price,
                Reference = article.Reference,
                CategoryId = article.CategoryId,
                Category = new CategoryResponseDto
                {
                    CategoryId = article.Category.CategoryId,
                    Name = article.Category.Name,
                    MenuId = article.Category.MenuId
                }
            };
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, ArticleRequestDto articleRequestDto)
        {
            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            // Check if the Category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == articleRequestDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("The specified CategoryId does not exist.");
            }
            var existingCategory = await _context.Categories.FindAsync(articleRequestDto.CategoryId);
            if (existingCategory == null)
            {
                return BadRequest("The specified CategoryId does not exist.");
            }
            // Update the article with the new values
            existingArticle.Name = articleRequestDto.Name;
            existingArticle.Price = articleRequestDto.Price;
            existingArticle.CategoryId = articleRequestDto.CategoryId;
            existingArticle.Reference = articleRequestDto.Reference;  // Update the reference

            _context.Entry(existingArticle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Articles.Any(a => a.ArticleId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ArticleResponseDto
            {
                ArticleId = existingArticle.ArticleId,
                Name = existingArticle.Name,
                Price = existingArticle.Price,
                CategoryId = existingArticle.CategoryId,
                Category = new CategoryResponseDto
                {
                    CategoryId = existingCategory.CategoryId,
                    Name = existingCategory.Name,
                    MenuId = existingCategory.MenuId
                },
                Reference = existingArticle.Reference  
            });
        }


        // POST: api/Articles
        [HttpPost]
        public async Task<ActionResult<ArticleResponseDto>> PostArticle(ArticleRequestDto articleRequestDto)
        {
            // Check if the referenced Category exists
            var category = await _context.Categories.FindAsync(articleRequestDto.CategoryId);
            if (category == null)
            {
                return NotFound($"Category with ID {articleRequestDto.CategoryId} not found.");
            }

            // Map the DTO to the Article entity, including the reference
            var article = new Article
            {
                Name = articleRequestDto.Name,
                Price = articleRequestDto.Price,
                CategoryId = articleRequestDto.CategoryId,
                Category = category,  // Set the navigation property
                Reference = articleRequestDto.Reference 
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return Ok(new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Name = article.Name,
                Price = article.Price,
                CategoryId = article.CategoryId,
                Category = new CategoryResponseDto
                {
                    CategoryId = article.Category.CategoryId,
                    Name = article.Category.Name,
                    MenuId = article.Category.MenuId
                },
                Reference = article.Reference  
            });
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Article deleted successfully." });
        }



        // GET: api/Articles with pagination
        [HttpGet("paginate")]
        public async Task<ActionResult<IEnumerable<ArticleResponseDto>>> GetArticlesPaginate([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
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

            var response = new
            {
                TotalArticles = totalArticlesCount,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalArticlesCount / (double)pageSize),
                Articles = articles
            };

            return Ok(response);
        }

    }
}
