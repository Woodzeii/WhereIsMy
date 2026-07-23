using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace WhereIsMy;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public LocationsController(IPublishEndpoint publishEndpoint, AppDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;

    }
    

  
    // СОЗДАНИЕ ЛОКАЦИИ
    [HttpPost("location")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request)
    {
        
        await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }
    // ОБНОВЛЕНИЕ ЛОКАЦИИ
    [HttpPut("location/{id}")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        // Логика обновления локации
         await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }

    //Удаление локации
    [HttpDelete("location/{id}")]
    public async Task<IActionResult> DeleteLocation([FromBody] DeleteLocationRequest request)
    {
        // Логика удаления локации
         await _publishEndpoint.Publish(request);
        
        return Accepted(new { message = "Запрос принят MassTransit" });
    }


    //ГЕТТЕРЫ-----------------------------------
    //1 ЛОКАЦИЯ
    [HttpGet("location/{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
       // 1. Ищем товар в базе данных (вместо _dbContext может быть ваш репозиторий)
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == id);

        // 2. Если товар не найден — сразу возвращаем 404 Not Found
        if (location == null)
        {
            return NotFound(new { message = $"Локация с ID {id} не найдена" });
        }

        // 3. Если найден — возвращаем 200 OK вместе с данными товара
        return Ok(location);
    }
    //Получить ВСЕ ЛОКАЦИИ
    [HttpGet("locations")]
    public async Task<IActionResult> GetAllLocations()
    {
        var locations = await _dbContext.Locations.ToListAsync();
        return Ok(locations);
    }
}

public record CreateLocationRequest(string Name, int UserId);
public record UpdateLocationRequest(int LocationId, string NewName, int UserId);
public record DeleteLocationRequest(int LocationId, int UserId);

