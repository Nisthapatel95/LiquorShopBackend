using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;

namespace LiquorShop.API.Services;

/// <summary>
/// Records a change event to the AuditLogs table.
/// Inject and call LogAsync() from any controller action that mutates data.
/// </summary>
public class AuditService
{
    private readonly AppDbContext _db;
    public AuditService(AppDbContext db) => _db = db;

    public async Task LogAsync(int userId, string action, string entity, int entityId,
                                string oldValues = "", string newValues = "")
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId    = userId,
            Action    = action,
            Entity    = entity,
            EntityId  = entityId,
            OldValues = oldValues,
            NewValues = newValues
        });
        await _db.SaveChangesAsync();
    }
}
