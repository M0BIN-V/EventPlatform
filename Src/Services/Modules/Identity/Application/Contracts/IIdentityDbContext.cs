using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts;

public interface IIdentityDbContext : IDbContextBase
{
    public DbSet<User> Users { get; set; }
}