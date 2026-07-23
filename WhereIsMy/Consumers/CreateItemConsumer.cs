using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;
public class CreateItemConsumer : IConsumer<CreateItemRequest>
{
    private readonly AppDbContext _dbContext;

    public CreateItemConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CreateItemRequest> context)
    {
        var message = context.Message;

        int? locationId = null;

        if (message.LocationId.HasValue)
        {
            locationId = await _dbContext.Locations
                .Where(x => x.Id == message.LocationId.Value && x.UserId == message.UserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
        }

        var newItem = new Item
        {
            Name = string.IsNullOrWhiteSpace(message.Name) ? "Без названия" : message.Name.Trim(),
            LocationId = locationId,
            UserId = message.UserId
        };

        _dbContext.Items.Add(newItem);
        await _dbContext.SaveChangesAsync();
    }
}
