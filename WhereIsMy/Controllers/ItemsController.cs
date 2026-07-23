using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ItemsController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    // ПЕРЕМЕЩЕНИЕ
    [HttpPut("move")]
    public async Task<IActionResult> MoveItem([FromBody] MoveItemRequest request)
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

    

    

}

public record MoveItemRequest(int ItemId, int NewLocationId, int UserId);
public record CreateItemRequest(string Name, int NewLocationId, int UserId);
