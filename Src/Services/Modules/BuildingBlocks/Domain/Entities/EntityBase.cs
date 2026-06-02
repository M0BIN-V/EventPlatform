namespace BuildingBlocks.Domain.Entities;

public abstract class EntityBase<TId>
{
    public TId Id { get; set; }
}

public abstract class EntityBase : EntityBase<Guid>;