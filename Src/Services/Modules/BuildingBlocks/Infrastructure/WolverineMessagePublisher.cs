using BuildingBlocks.Application.Contracts;
using Wolverine;

namespace BuildingBlocks.Infrastructure;

public class WolverineMessagePublisher(IMessageBus messageBus) : IMessagePublisher
{
    public async Task PublishAsync<TMessage>(TMessage message)
    {
        await messageBus.PublishAsync(message);
    }
}