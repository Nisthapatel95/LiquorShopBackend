using LiquorShop.API.Data;
using LiquorShop.API.Data.Entities;
using LiquorShop.API.DTOs;
using LiquorShop.API.Helpers;
using LiquorShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiquorShop.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings  _jwt;

    public AuthService(AppDbContext db, JwtSettings jwt)
    {
        _db  = db;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = JwtHelper.GenerateToken(user, _jwt);
        return new LoginResponse(token, user.FullName, user.Role.Name, user.Id);
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return false;

        var user = new User
        {
            FullName     = request.FullName,
            Email        = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId       = request.RoleId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
