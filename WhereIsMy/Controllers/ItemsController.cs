using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ItemsController(IPublishEndpoint publishEndpoint, AppDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
    }

    [HttpPut("item/{id}")]
    public async Task<IActionResult> ChangeItem([FromBody] СhangeItemRequest request)
    {
        var userId = GetCurrentUserId();
        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    [HttpPost("item")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
    {
        var userId = GetCurrentUserId();
        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    [HttpDelete("item/{id}")]
    public async Task<IActionResult> DeleteItem([FromBody] DeleteItemRequest request)
    {
        var userId = GetCurrentUserId();
        await _publishEndpoint.Publish(request with { UserId = userId });
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    //ГЕТТЕРЫ-----------------------------------
    //1 ВЕЩЬ
    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        // 1. Ищем товар в базе данных (вместо _dbContext может быть ваш репозиторий)
        var item = await _dbContext.Items
            .FirstOrDefaultAsync(x => x.Id == id);

        // 2. Если товар не найден — сразу возвращаем 404 Not Found
        if (item == null)
        {
            return NotFound(new { message = $"Товар с ID {id} не найден" });
        }

        // 3. Если найден — возвращаем 200 OK вместе с данными товара
        return Ok(item);
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems()
    {
        var userId = GetCurrentUserId();
        var items = await _dbContext.Items
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return Ok(items);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.Parse(claim ?? throw new UnauthorizedAccessException("Пользователь не найден в токене"));
    }
}

public record СhangeItemRequest(int ItemId, string NewName, int NewLocationId, int UserId);
public record CreateItemRequest(string Name, int NewLocationId, int UserId);

public record DeleteItemRequest(int ItemId, int UserId);
