using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Organization.Application.Common.Contracts.Persistence;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure;

public static class InfrastructureServiceInstaller
{
    public static IHostApplicationBuilder AddOrganizationModuleInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfOrganizationDbContext>(
            "event-platform-db",
            null,
            dbContextOpt => dbContextOpt.UseNpgsql(npgOpt => npgOpt
                .MigrationsHistoryTable("__EFMigrationsHistory", EfOrganizationDbContext.Schema)));

        // Register repositories and unit of work
        builder.Services
            .AddScoped<IOrganizationUnitOfWork, OrganizationUnitOfWork>();

        return builder;
    }
}
