using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace WhereIsMy;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public LocationsController(IPublishEndpoint publishEndpoint, AppDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;

    }
    

  
    [HttpPost("location")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request)
    {
        var userId = GetCurrentUserId();
        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    [HttpPut("location/{id}")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        var userId = GetCurrentUserId();

        var locationExists = await _dbContext.Locations
            .AnyAsync(x => x.Id == request.LocationId && x.UserId == userId);

        if (!locationExists)
        {
            return NotFound(new { message = "Локация не найдена или не принадлежит текущему пользователю" });
        }

        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    [HttpDelete("location/{id}")]
    public async Task<IActionResult> DeleteLocation([FromBody] DeleteLocationRequest request)
    {
        var userId = GetCurrentUserId();

        var locationExists = await _dbContext.Locations
            .AnyAsync(x => x.Id == request.LocationId && x.UserId == userId);

        if (!locationExists)
        {
            return NotFound(new { message = "Локация не найдена или не принадлежит текущему пользователю" });
        }

        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }


    //ГЕТТЕРЫ-----------------------------------
    //1 ЛОКАЦИЯ
    [HttpGet("location/{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
        var userId = GetCurrentUserId();

        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (location == null)
        {
            return NotFound(new { message = $"Локация с ID {id} не найдена или не принадлежит текущему пользователю" });
        }

        return Ok(location);
    }
    [HttpGet("locations")]
    public async Task<IActionResult> GetAllLocations()
    {
        var userId = GetCurrentUserId();
        var locations = await _dbContext.Locations
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return Ok(locations);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.Parse(claim ?? throw new UnauthorizedAccessException("Пользователь не найден в токене"));
    }
}

public record CreateLocationRequest(string Name, int UserId);
public record UpdateLocationRequest(int LocationId, string NewName, int UserId);
public record DeleteLocationRequest(int LocationId, int UserId);

