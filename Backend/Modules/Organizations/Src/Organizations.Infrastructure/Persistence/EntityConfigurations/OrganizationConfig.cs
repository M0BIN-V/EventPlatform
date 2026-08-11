using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organizations.Infrastructure.Persistence.EntityConfigurations;

public class OrganizationConfig : IEntityTypeConfiguration<Organizations.Domain.Entities.Organization>
{
    public void Configure(EntityTypeBuilder<Organizations.Domain.Entities.Organization> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatorUserId)
            .IsRequired()
            .HasMaxLength(450); // IdentityUser Id max length

        // Unique constraint on slug
        builder.HasIndex(x => x.Slug)
            .IsUnique();

        // Navigation
        builder.HasMany(x => x.Members)
            .WithOne(x => x.Organization)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}