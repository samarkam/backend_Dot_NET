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
    public class MenusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Menus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuResponseDto>>> GetMenus()
        {
            var menus = await _context.Menus
                .Include(m => m.Categorys)  // Include categories as DTOs
                .Select(m => new MenuResponseDto
                {
                    MenuId = m.MenuId,
                    Name = m.Name,
                    Description = m.Description,
                    Categories = m.Categorys
                        .Select(c => new CategoryResponseDto
                        {
                            CategoryId = c.CategoryId,
                            Name = c.Name,
                            MenuId = c.MenuId,
                            Articles = c.Articles
                                .Where(a => a.IsVisible)  // Filter only visible articles
                                .Select(a => new ArticleResponseDto
                                {
                                    ArticleId = a.ArticleId,
                                    Name = a.Name,
                                    Price = a.Price,
                                    IsVisible = a.IsVisible,
                                    CategoryId = a.CategoryId,
                                    Reference = a.Reference
                                }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            return Ok(menus);
        }



        // GET: api/Menus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuResponseDto>> GetMenu(int id)
        {
            var menu = await _context.Menus
                .Include(m => m.Categorys)  // Include categories as DTOs
                .Where(m => m.MenuId == id)
                .Select(m => new MenuResponseDto
                {
                    MenuId = m.MenuId,
                    Name = m.Name,
                    Description = m.Description,
                    Categories = m.Categorys.Select(c => new CategoryResponseDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        MenuId = c.MenuId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (menu == null)
            {
                return NotFound();
            }

            return Ok(menu);
        }


        // PUT: api/Menus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // PUT: api/Menus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu(int id, MenuRequestDto menuRequestDto)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            // Update the menu properties
            menu.Name = menuRequestDto.Name;
            menu.Description = menuRequestDto.Description;

            _context.Entry(menu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new MenuResponseDto
            {
                MenuId = menu.MenuId,
                Name = menu.Name,
                Description = menu.Description,
                
            });
        }


      // POST: api/Menus
        [HttpPost]
        public async Task<ActionResult<Menu>> PostMenu(MenuRequestDto menuDto)
        {
            // Map the DTO to the entity
            var menu = new Menu
            {
                Name = menuDto.Name,
                Description = menuDto.Description,
                Categorys = new List<Category>() 
            };

            // Add the entity to the database context
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            // Return the created menu with a reference to the GetMenu action
            return CreatedAtAction("GetMenu", new { id = menu.MenuId }, menu);
        }

        // DELETE: api/Menus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu deleted successfully." });
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.MenuId == id);
        }
    }
}
