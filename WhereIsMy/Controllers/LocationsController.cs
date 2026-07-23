using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public LocationsController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

  
    // СОЗДАНИЕ ЛОКАЦИИ
    [HttpPost("location")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request)
    {
        
        await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }
    [HttpPut("location/{id}")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        // Логика обновления локации
         await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }
}

public record CreateLocationRequest(string Name, int UserId);
public record UpdateLocationRequest(int Id, string NewName);

