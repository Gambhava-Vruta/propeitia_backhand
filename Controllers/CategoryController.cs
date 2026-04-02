using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly PropertiaContext _db;

    public CategoriesController(PropertiaContext db)
    {
        _db = db;
    }

    // =========================
    // GET: api/categories
    // =========================
    [Authorize(Roles = "admin,buyer,seller")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _db.Categories
                .Select(c => new Categorydto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting categories",
                error = ex.Message
            });
        }
    }

    // =========================
    // GET: api/categories/5
    // =========================
    [Authorize(Roles = "admin,buyer,seller")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var category = await _db.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new Categorydto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting category",
                error = ex.Message
            });
        }
    }

    // =========================
    // POST: api/categories
    // =========================
    [Authorize(Roles = "admin,seller")]
    [HttpPost]
    public async Task<IActionResult> Create(Categorydto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _db.Categories
                .AnyAsync(c => c.CategoryName == dto.CategoryName);

            if (exists)
                return BadRequest(new { message = "Category already exists" });

            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return Created("", new
            {
                category.CategoryId,
                category.CategoryName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error creating category",
                error = ex.Message
            });
        }
    }

    // =========================
    // PUT: api/categories/5
    // =========================
    [Authorize(Roles = "admin,seller")]

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Categorydto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _db.Categories.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Category not found" });

            bool exists = await _db.Categories
                .AnyAsync(c => c.CategoryName == dto.CategoryName && c.CategoryId != id);

            if (exists)
                return BadRequest(new { message = "Category name already exists" });

            existing.CategoryName = dto.CategoryName;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error updating category",
                error = ex.Message
            });
        }
    }

    // =========================
    // DELETE: api/categories/5
    // =========================
    [Authorize(Roles = "admin,seller")]

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Optional: prevent delete if category is in use
            bool inUse = await _db.Properties
                .AnyAsync(p => p.CategoryId == id);

            if (inUse)
                return BadRequest(new
                {
                    message = "Cannot delete category because it is in use"
                });

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error deleting category",
                error = ex.Message
            });
        }
    }
}
