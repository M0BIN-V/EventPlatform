using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Organizations.Infrastructure.Persistence;
using Organizations.Infrastructure.Persistence.Repositories;

namespace Organizations.Infrastructure;

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
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationUnitOfWork, OrganizationUnitOfWork>();

        return builder;
    }
}