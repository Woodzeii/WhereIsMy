using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WhereIsMy;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Login) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Имя, логин и пароль обязательны" });
        }

        if (await _dbContext.Users.AnyAsync(x => x.Login == request.Login))
        {
            return Conflict(new { message = "Пользователь с таким логином уже существует" });
        }

        var user = new User
        {
            Name = request.Name,
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse(user.Id, user.Name, user.Login, token));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Логин и пароль обязательны" });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == request.Login);
        if (user == null)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return Unauthorized(new { message = "Неверный логин или пароль" });
        }

        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse(user.Id, user.Name, user.Login, token));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetCurrentUserId();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        var locations = await _dbContext.Locations
            .Where(x => x.UserId == userId)
            .Select(x => new { id = x.Id, name = x.Name })
            .ToListAsync();

        var items = await _dbContext.Items
            .Where(x => x.UserId == userId)
            .Select(x => new { id = x.Id, name = x.Name, locationId = x.LocationId })
            .ToListAsync();

        return Ok(new
        {
            id = user.Id,
            name = user.Name,
            login = user.Login,
            locations,
            items
        });
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.Parse(claim ?? throw new UnauthorizedAccessException("Пользователь не найден в токене"));
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WhereIsMy";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "WhereIsMyClient";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Login),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Name, string Login, string Password);
public record LoginRequest(string Login, string Password);
public record AuthResponse(int Id, string Name, string Login, string Token);
