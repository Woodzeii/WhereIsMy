using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;
public class CreateItemConsumer : IConsumer<СhangeItemRequest>
{
    private readonly AppDbContext _dbContext;

    public CreateItemConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<СhangeItemRequest> context)
    {
        var message = context.Message;

        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == message.NewLocationId && x.UserId == message.UserId);

        if (location is null)
        {
            throw new InvalidOperationException($"Location with ID {message.NewLocationId} was not found for user {message.UserId}.");
        }

        var newItem = new Item
        {
            Name = message.NewName,
            LocationId = location.Id,
            UserId = message.UserId
        };

        _dbContext.Items.Add(newItem);
        await _dbContext.SaveChangesAsync();
    }
}
