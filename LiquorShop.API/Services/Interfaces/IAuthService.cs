using LiquorShop.API.DTOs;

namespace LiquorShop.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool>           RegisterAsync(RegisterRequest request);
}
