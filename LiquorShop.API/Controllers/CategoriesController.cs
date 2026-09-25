using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Controllers;

/// <summary>Product category management.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    /// <summary>List all categories.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cats = await _db.Categories
            .Select(c => new CategoryDto
            {
                Id          = c.Id,
                Name        = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
        return Ok(cats);
    }

    /// <summary>Create a new category.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var cat = new Category { Name = dto.Name, Description = dto.Description };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return Ok(new CategoryDto { Id = cat.Id, Name = cat.Name, Description = cat.Description, ProductCount = 0 });
    }

    /// <summary>Update a category.</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        var cat = await _db.Categories.FindAsync(id);
        if (cat is null) return NotFound();
        cat.Name        = dto.Name;
        cat.Description = dto.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Delete a category (only if no products linked).</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (cat is null) return NotFound();
        if (cat.Products.Any())
            return BadRequest(new { message = "Cannot delete category with existing products." });
        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
