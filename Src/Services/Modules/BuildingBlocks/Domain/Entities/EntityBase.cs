namespace BuildingBlocks.Domain.Entities;

public abstract class EntityBase<TId>
{
    public TId Id { get; set; } = default!;
}

public abstract class EntityBase : EntityBase<Guid>;