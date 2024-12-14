using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.models;
using backend.DTO;
using backend.REPOSITORY;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(ApplicationDbContext context, IArticleRepository articleRepository)
        {
            _context = context;
            _articleRepository = articleRepository;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleResponseDto>>> GetArticles()
        {
            var articles = await _articleRepository.GetArticlesAsync();
            return Ok(articles);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleResponseDto>> GetArticle(int id)
        {
            var article = await _articleRepository.GetArticleByIdAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }
        [HttpPost("visible")]
        public async Task<IActionResult> UpdateVisibility([FromBody] UpdateVisibilityDto updateVisibilityDto)
        {
            var article = await _context.Articles.FindAsync(updateVisibilityDto.ArticleId);
            if (article == null)
            {
                return NotFound();
            }

            article.IsVisible = updateVisibilityDto.IsVisible;
            await _articleRepository.UpdateArticleAsyncVisibility(article);  
            return Ok("Visibility updated successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, ArticleRequestDto articleRequestDto)
        {
            try
            {
                var updatedArticle = await _articleRepository.UpdateArticleAsync(id, articleRequestDto);
                if (updatedArticle == null)
                {
                    return NotFound(); // Article not found
                }

                return Ok(updatedArticle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Handle invalid CategoryId
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "A concurrency error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ArticleResponseDto>> PostArticle(ArticleRequestDto articleRequestDto)
        {
            try
            {
                var createdArticle = await _articleRepository.CreateArticleAsync(articleRequestDto);
                return Ok(createdArticle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Handle invalid CategoryId
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await _articleRepository.DeleteArticleAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Article with ID {id} not found." });
            }

            return Ok(new { message = "Article deleted successfully." });
        }



        [HttpGet("paginate")]
        public async Task<ActionResult> GetArticlesPaginate([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
            }

            var (articles, totalArticlesCount) = await _articleRepository.GetArticlesPaginateAsync(page, pageSize);

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
