using MassTransit;
using Microsoft.EntityFrameworkCore;
using WhereIsMy;

public class DeleteItemConsumer : IConsumer<DeleteItemRequest>
{
    private readonly AppDbContext _dbContext;

    public DeleteItemConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DeleteItemRequest> context)
    {
        var message = context.Message;

        var item = await _dbContext.Items
            .FirstOrDefaultAsync(x => x.Id == message.ItemId && x.UserId == message.UserId);

        if (item is null)
        {
            throw new InvalidOperationException($"Item with ID {message.ItemId} was not found for user {message.UserId}.");
        }

        _dbContext.Items.Remove(item);
        await _dbContext.SaveChangesAsync();
    }
}