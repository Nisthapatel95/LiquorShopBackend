using LiquorShop.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Controllers;

/// <summary>Audit trail — Admin only.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuditLogsController(AppDbContext db) => _db = db;

    /// <summary>List all audit log entries, newest first.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _db.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Action,
                a.Entity,
                a.EntityId,
                a.OldValues,
                a.NewValues,
                a.CreatedAt,
                UserName = a.User.FullName
            })
            .ToListAsync();

        return Ok(logs);
    }
}
