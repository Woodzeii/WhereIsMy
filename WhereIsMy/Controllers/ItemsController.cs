using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ItemsController(IPublishEndpoint publishEndpoint, AppDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
    }

    // Передел
    [HttpPut("item/{id}")]
    public async Task<IActionResult> ChangeItem([FromBody] СhangeItemRequest request)
    {
        // MassTransit сам упакует объект в JSON, байты и отправит в RabbitMQ
        await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    // СОЗДАНИЕ
    [HttpPost("item")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
    {
        // MassTransit сам упакует объект в JSON, байты и отправит в RabbitMQ
        await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    //Удаление
    [HttpDelete("item/{id}")]
    public async Task<IActionResult> DeleteItem([FromBody] DeleteItemRequest request)
    {
        // MassTransit сам упакует объект в JSON, байты и отправит в RabbitMQ
        await _publishEndpoint.Publish(request);
        
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

    //Получить ВСЕ ВЕЩИ
    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _dbContext.Items.ToListAsync();
        return Ok(items);
    }



}

public record СhangeItemRequest(int ItemId, string NewName, int NewLocationId, int UserId);
public record CreateItemRequest(string Name, int NewLocationId, int UserId);

public record DeleteItemRequest(int ItemId, int UserId);
