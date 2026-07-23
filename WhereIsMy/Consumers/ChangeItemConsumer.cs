using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;
public class ChangeItemConsumer : IConsumer<ChangeItemRequest>
{
    private readonly AppDbContext _dbContext;

    public ChangeItemConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ChangeItemRequest> context)
    {
        var message = context.Message;

        var item = await _dbContext.Items
            .FirstOrDefaultAsync(x => x.Id == message.ItemId && x.UserId == message.UserId);

        if (item is null)
        {
            throw new InvalidOperationException($"Item with ID {message.ItemId} was not found for user {message.UserId}.");
        }

        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == message.NewLocationId && x.UserId == message.UserId);

        if (location is null)
        {
            throw new InvalidOperationException($"Location with ID {message.NewLocationId} was not found for user {message.UserId}.");
        }

        item.Name = message.NewName;
        item.LocationId = location.Id;
        item.UserId = message.UserId;

        await _dbContext.SaveChangesAsync();
    }
}
