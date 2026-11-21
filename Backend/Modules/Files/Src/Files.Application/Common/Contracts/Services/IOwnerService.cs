using Files.Domain.Entities;

namespace Files.Application.Common.Contracts.Services;

public interface IOwnerService
{
    public IEnumerable<Owner> GetOwners();
    public Owner? GetOwner(string ownerId);
}