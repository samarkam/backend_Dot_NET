using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.models;
using backend.DTO;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
        {
            var categories = await _context.Categories
                                           .Select(c => new CategoryResponseDto
                                           {
                                               CategoryId = c.CategoryId,
                                               Name = c.Name,
                                               MenuId = c.MenuId
                                           })
                                           .ToListAsync();

            return categories;
        }


        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                MenuId = category.MenuId
            };
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryRequestDto categoryRequest)
        {
            // Check if the category exists
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            // Check if the Menu exists
            var menuExists = await _context.Menus.AnyAsync(m => m.MenuId == categoryRequest.MenuId);
            if (!menuExists)
            {
                return BadRequest("The specified MenuId does not exist.");
            }

            // Update the category with the new values
            existingCategory.Name = categoryRequest.Name;
            existingCategory.MenuId = categoryRequest.MenuId;

            _context.Entry(existingCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(categoryRequest);
        }


        // POST: api/Categories
        

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryRequestDto categoryDto)
        {
            // Check if the referenced Menu exists
            var menu = await _context.Menus.FindAsync(categoryDto.MenuId);
            if (menu == null)
            {
                return NotFound($"Menu with ID {categoryDto.MenuId} not found.");
            }

            // Map the DTO to the Category entity
            var category = new Category
            {
                Name = categoryDto.Name,
                MenuId = categoryDto.MenuId,
                Menu = menu ,
                Articles = new List<Article>()

            };

            // Add the entity to the database context
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Return the created category with a reference to the GetCategory action
            return Ok( categoryDto);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "category deleted successfully." });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
