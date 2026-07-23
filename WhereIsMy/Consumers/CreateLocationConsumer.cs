using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WhereIsMy;
public class CreateLocationConsumer : IConsumer<CreateLocationRequest>
{
    private readonly AppDbContext _dbContext;

    public CreateLocationConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CreateLocationRequest> context)
    {
        var message = context.Message;

        var newLocation = new Location
        {
            Name = message.Name,
            UserId = message.UserId
        };

        _dbContext.Locations.Add(newLocation);
        await _dbContext.SaveChangesAsync();
    }
}
