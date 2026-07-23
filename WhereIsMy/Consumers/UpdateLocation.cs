using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;
public class UpdateLocationConsumer : IConsumer<UpdateLocationRequest>
{
    private readonly AppDbContext _dbContext;

    public UpdateLocationConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UpdateLocationRequest> context)
    {
        var message = context.Message;

        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == message.LocationId && x.UserId == message.UserId);

        if (location is null)
        {
            throw new InvalidOperationException($"Location with ID {message.LocationId} was not found for user {message.UserId}.");
        }

        // var newLocation = await _dbContext.Locations
        //     .FirstOrDefaultAsync(x => x.Id == message.LocationId && x.UserId == message.UserId);

       

        location.Name = message.NewName;
        

        await _dbContext.SaveChangesAsync();
    }
}
