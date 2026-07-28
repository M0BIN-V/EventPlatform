using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Infrastructure.Persistence.DbContext;

// public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EfIdentityDbContext>
// {
//     public EfIdentityDbContext CreateDbContext(string[] args)
//     {
//         var optionsBuilder = new DbContextOptionsBuilder<EfIdentityDbContext>();
//
//         optionsBuilder.UseNpgsql();
//
//         return new EfIdentityDbContext(optionsBuilder.Options);
//     }
// }