using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Controllers;

/// <summary>Supplier management.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _db;
    public SuppliersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Suppliers.Where(s => s.IsActive).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Suppliers.FindAsync(id);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name        = dto.Name,
            ContactName = dto.ContactName,
            Phone       = dto.Phone,
            Email       = dto.Email,
            Address     = dto.Address
        };
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateSupplierDto dto)
    {
        var supplier = await _db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();

        supplier.Name        = dto.Name;
        supplier.ContactName = dto.ContactName;
        supplier.Phone       = dto.Phone;
        supplier.Email       = dto.Email;
        supplier.Address     = dto.Address;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();
        supplier.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
