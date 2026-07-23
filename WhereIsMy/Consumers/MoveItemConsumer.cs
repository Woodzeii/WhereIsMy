using MassTransit;

public class MoveItemConsumer : IConsumer<MoveItemRequest>
{
    public async Task Consume(ConsumeContext<MoveItemRequest> context)
    {
        var message = context.Message;
        
        // Здесь пишется любая тяжелая бизнес-логика
        Console.WriteLine($"[Бизнес-логика] Перемещаем товар {message.ItemId} в локацию {message.NewLocationId}");
        
        await Task.CompletedTask;
    }
}
