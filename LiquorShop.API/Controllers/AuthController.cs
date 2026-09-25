using LiquorShop.API.DTOs;
using LiquorShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiquorShop.API.Controllers;

/// <summary>Authentication — login and register.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return result is null ? Unauthorized(new { message = "Invalid credentials." }) : Ok(result);
    }

    /// <summary>Register a new user (Admin only in production).</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var success = await _auth.RegisterAsync(request);
        return success ? Ok(new { message = "User registered." }) : Conflict(new { message = "Email already exists." });
    }
}
