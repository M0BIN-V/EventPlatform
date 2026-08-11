namespace BuildingBlocks.Domain.Entities;

public abstract class EntityBase<TId>
{
    public virtual TId Id { get; init; } = default!;
}

public abstract class EntityBase : EntityBase<Guid>
{
    public override Guid Id { get; init; } = Guid.CreateVersion7();
}