namespace LiquorShop.API.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, string FullName, string Role, int UserId);

public record RegisterRequest(string FullName, string Email, string Password, int RoleId);
