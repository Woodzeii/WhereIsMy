using MassTransit;
using Microsoft.EntityFrameworkCore;
using WhereIsMy;

public class DeleteLocationConsumer : IConsumer<DeleteLocationRequest>
{
    private readonly AppDbContext _dbContext;

    public DeleteLocationConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DeleteLocationRequest> context)
    {
        var message = context.Message;

        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == message.LocationId && x.UserId == message.UserId);

        if (location is null)
        {
            throw new InvalidOperationException($"Location with ID {message.LocationId} was not found for user {message.UserId}.");
        }

        _dbContext.Locations.Remove(location);
        await _dbContext.SaveChangesAsync();
    }
}