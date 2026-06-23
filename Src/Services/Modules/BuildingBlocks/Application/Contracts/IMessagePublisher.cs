namespace BuildingBlocks.Application.Contracts;

public interface IMessagePublisher
{
    public Task PublishAsync<TMessage>(TMessage message);
}