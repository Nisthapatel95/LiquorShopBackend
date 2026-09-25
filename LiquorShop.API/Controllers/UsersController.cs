using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace LiquorShop.API.Controllers;

/// <summary>User management — Admin only.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    /// <summary>List all users.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Include(u => u.Role)
            .OrderBy(u => u.FullName)
            .Select(u => new UserDto
            {
                Id       = u.Id,
                FullName = u.FullName,
                Email    = u.Email,
                Role     = u.Role.Name,
                RoleId   = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
        return Ok(users);
    }

    /// <summary>Create a new user (cashier or admin).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email already in use." });

        var user = new User
        {
            FullName     = dto.FullName,
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId       = dto.RoleId,
            IsActive     = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { user.Id, user.FullName, user.Email });
    }

    /// <summary>Toggle user active/inactive.</summary>
    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();
        return Ok(new { user.Id, user.IsActive });
    }

    /// <summary>Reset user password.</summary>
    [HttpPut("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Password reset successfully." });
    }
}
